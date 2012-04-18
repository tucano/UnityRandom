using System;
using System.Collections;
using System.Linq;
using NPack;

namespace URandom
{
	public class DiceRoll
	{
		// STORE DICE RESULTS in an ARRAYLIST
		private int[] _result;
		public int[] result
		{
			get
			{
				return _result;
			}
		}
		
		public enum DiceType
		{
			D2 = 2,
			D3 = 3,
			D4 = 4,
			D6 = 6,
			D8 = 8,
			D10 = 10,
			D12 = 12,
			D20 = 20,
			D30 = 30,
			D100 = 100
		}
		
		private DiceType _dice_type = DiceType.D6;
		public DiceType type
		{
			get { return _dice_type; }
		}
		
		private int _size = 1;
		public int size
		{
			get { return _size; }
		}
		
		// CONSTRUCTOR
		public DiceRoll(int size, DiceType type, ref NPack.MersenneTwister _rand)
		{
			if (size < 1) throw new ArgumentOutOfRangeException ("Number of dices shlud be > 0");
			init(size,type,ref _rand);
		}
		
		// INIT
		private void init(int size, DiceType type, ref NPack.MersenneTwister _rand)
		{
			_result = new int[size];
			_dice_type = type;
			_size = size;
			
			for (int i = 0; i < _size; i++) {
				// Cast enum to int to get the value
				_result[i] = _rand.Next( 1, (int) type);
			}
		}
		
		// DICETYPE TO STRING
		public String TypeToString()
		{
			return _size + _dice_type.ToString();
		}
		
		// DICEROLL TO STRING
		public String RollToString()
		{
			String s = "";
			for (int i = 0; i < _size; i++) {
				s += _result[i].ToString();
				if (i != (_size -1)) s += ", ";
			}
			return s;
		}
		
		public int Sum()
		{
			return _result.Sum();
		}
		
	}
}