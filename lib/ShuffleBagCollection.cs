/*
 * Adapted from: http://liboxide.svn.sourceforge.net/viewvc/liboxide/trunk/Oxide/Collections/ShuffleBagCollection.cs?view=markup
 * 
 * davide.rambaldi AT gmail.com : Changed Random to Mersenne Twister random, changed namespace to URandom
 * 
 */

/*
 * Oxide .NET extensions library
 * Copyright (C) 2009 Matthew Scharley and the liboxide team
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as 
 * published by the Free Software Foundation, either version 3 of the 
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

/*
 * This class is a port and/or based very heavily on the GShuffleBag class 
 * written by Jos Hirth <http://kaioa.com/node/53> originally implemented
 * in Java.
 */

/*
 * This file is also released seperately from the Oxide Library under a 
 * BSD style licence described at <http://kaioa.com/node/31>, with the
 * following extra provision:
 * 
 * - All the above copyright notices must be kept intact in any source
 *   code derived from this file.
 */

/*
 * Works with any class that subclasses System.Random
 * 
 * There is a Mersenne Twister implementation available from
 * http://code.msdn.microsoft.com/MersenneTwister if the default
 * implementation is not good enough.
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NPack;

namespace URandom
{
    public sealed class ShuffleBagCollection<T> : IEnumerable<T>
    {
        private Random m_generator;
        private List<T> m_data;
        private int m_cursor = -1;
        private T m_current = default(T);

        /// <summary>
        /// Constructs an empty bag with an initial capacity of 10 and the default source of randomness.
        /// </summary>
        public ShuffleBagCollection() : this(10, new MersenneTwister()) { }
        /// <summary>
        /// Constructs an empty bag with an initial capacity of 10 and the specified source of randomness.
        /// </summary>
        /// <param name="generator">The random number generator to use</param>
        public ShuffleBagCollection(MersenneTwister generator) : this(10, generator) { }
        /// <summary>
        /// Constructs an empty bag with the specified initial capacity and the default source of randomness.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity to use</param>
        /// <exception cref="System.ArgumentException">Thrown if initialCapacity &lt; 0</exception>
        public ShuffleBagCollection(int initialCapacity) : this(initialCapacity, new MersenneTwister()) { }
        /// <summary>
        /// Creates an empty bag with the specified initial capacity and the specified random number generator.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity to use</param>
        /// <param name="generator">The random number generator to use</param>
        /// <exception cref="System.ArgumentException">Thrown if initialCapacity &lt; 0</exception>
        public ShuffleBagCollection(int initialCapacity, MersenneTwister generator)
        {
            if (initialCapacity < 0)
                throw new ArgumentException("Capacity must be a positive integer.", "initialCapacity");

            m_generator = generator;
            m_data = new List<T>(initialCapacity);
        }

        /// <summary>
        /// Add an item to the bag once.
        /// </summary>
        /// <param name="item">The item to throw into the bag.</param>
        public void Add(T item)
        { Add(item, 1); }
        /// <summary>
        /// Adds an item to the bag multiple times.
        /// </summary>
        /// <param name="item">The item to throw into the bag.</param>
        /// <param name="quantity">The number of times it should come back out.</param>
        /// <exception cref="System.ArgumentException">Thrown if quantity is not &gt; 0</exception>
        public void Add(T item, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be a positive integer.", "quantity");

            for (int i = 0; i < quantity; i++)
            {
                m_data.Add(item);
            }

            // Reseting the cursor to the end makes it possible to get freshly added values right away,
            // otherwise it would have to finish this run first.
            m_cursor = m_data.Count - 1;
        }

        /// <summary>
        /// Pulls an item out of the bag.
        /// </summary>
        /// <returns>The next item in the sequence.</returns>
        public T Next()
        {
            if (m_cursor < 1)
            {
                m_cursor = m_data.Count - 1;
                m_current = m_data[0];
                return m_current;
            }

            int index = m_generator.Next(m_cursor);
            m_current = m_data[index];
            m_data[index] = m_data[m_cursor];
            m_data[m_cursor] = m_current;
            m_cursor--;
            return m_current;
        }

        /// <summary>
        /// The last element that was returned from Next(). Can be null, if Next() has not been called
        /// yet.
        /// </summary>
        public T Current
        {
            get { return m_current; }
        }

        /// <summary>
        /// The current capacity of the underlying storage.
        /// </summary>
        public int Capacity
        {
            get { return m_data.Capacity; }
        }

        /// <summary>
        /// Reduces the capacity as much as possible to save memory.
        /// </summary>
        public void TrimExcess()
        {
            m_data.TrimExcess();
        }

        /// <summary>
        /// The number of elements in this bag.
        /// </summary>
        public int Size
        {
            get { return m_data.Count; }
        }

        #region IEnumerable<T> Members

        /// <summary>
        /// </summary>
        /// <returns>A sequence of random elements from the bag.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i <= Size; i++)
            {
                yield return this.Next();
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// </summary>
        /// <returns>A sequence of random elements from the bag.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        #endregion
    }
}