using System;
using System.Text;

namespace CommonHelper
{
	public static class IdentityCard
	{
		public static IIDCard Analye(string cardNumber)
		{
			cardNumber = cardNumber ?? string.Empty;
			switch(cardNumber.Length)
			{
				case 15:
					return new ChinaCardLength15(cardNumber);
					break;
				default:
					return new ChinaCardLength18(cardNumber);
			}
		}
	}

	internal class ChinaCardLength15 : IIDCard
	{
		private int m_Validated = -1;
		private string m_CardNumber = string.Empty;

		private string m_ProvinceCode = string.Empty;
		private string m_CityCode = string.Empty;
		private string m_DistrictCode = string.Empty;
		
		private DateTime m_Birthday = DateTime.MaxValue;
		private int m_Sex = -1;
		private static DateTime[] s_Range = new DateTime[]{ new DateTime(1900,1,1), new DateTime(2000,1,1) };

		
		public ChinaCardLength15(string cardNumber)
		{
			this.m_CardNumber = (cardNumber ?? string.Empty).ToUpper();
			if(this.m_CardNumber.Length != 15)
			{
				this.m_Validated = 0;
			}
		}
		
		#region IIdentityCardAnalyser 成员

		public string Province
		{
			get
			{ 
				if(this.m_ProvinceCode == null)
				{
					this.m_ProvinceCode = this.m_CardNumber.Substring(0,2);
				}
				return this.m_ProvinceCode;
			}
		}

		public string City
		{
			get
			{ 
				if(this.m_CityCode == null)
				{
					this.m_CityCode = this.m_CardNumber.Substring(0,4) + "00";
				}
				return this.m_CityCode;
			}
		}

		public string District
		{
			get
			{
				if(this.m_DistrictCode == null)
				{
					this.m_DistrictCode = this.m_CardNumber.Substring(0,6);
				}
				return this.m_DistrictCode;
			}
		}

		public DateTime Birthday
		{
			get
			{
				if(this.m_Birthday == DateTime.MaxValue)
				{
					if(! DateTime.TryParse("19" + m_CardNumber.Substring(6,2) + "-" + m_CardNumber.Substring(8,2) + "-" + m_CardNumber.Substring(10,2), out this.m_Birthday))
					{
						this.m_Birthday = DateTime.MinValue;
					}
				}
				return this.m_Birthday;
			}
		}

		public int Sex
		{
			get
			{
				if(this.m_Sex == -1)
				{
					this.m_Sex = this.m_CardNumber[14] % 2;
				}
				return this.m_Sex;
			}
		}

		public bool ValidatedSuccess
		{
			get
			{
				if(this.m_Validated == -1)
				{
					this.m_Validated = (this.Birthday >= s_Range[0] && this.Birthday < s_Range[1]) ? 1 : 0;
				}
				return this.m_Validated.Equals(1);
			}
		}

		#endregion
	}

	internal class ChinaCardLength18 : IIDCard
	{
		private static int[] s_Wi = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
		private static string s_LastCode = "10X98765432";

		private int m_Validated = -1;
		private string m_CardNumber = string.Empty;

		private string m_ProvinceCode = string.Empty;
		private string m_CityCode = string.Empty;
		private string m_DistrictCode = string.Empty;

		private DateTime m_Birthday = DateTime.MaxValue;
		private int m_Sex = -1;
		private static DateTime[] s_Range = new DateTime[] { new DateTime(1900, 1, 1), DateTime.MaxValue };


		public ChinaCardLength18(string cardNumber)
		{
			this.m_CardNumber = (cardNumber ?? string.Empty).ToUpper();
			if (this.m_CardNumber.Length != 18)
			{
				this.m_Validated = 0;
			}
		}

		#region IIdentityCardAnalyser 成员

		public string Province
		{
			get
			{
				if (this.m_ProvinceCode == null)
				{
					this.m_ProvinceCode = this.m_CardNumber.Substring(0, 2);
				}
				return this.m_ProvinceCode;
			}
		}

		public string City
		{
			get
			{
				if (this.m_CityCode == null)
				{
					this.m_CityCode = this.m_CardNumber.Substring(0, 4) + "00";
				}
				return this.m_CityCode;
			}
		}

		public string District
		{
			get
			{
				if (this.m_DistrictCode == null)
				{
					this.m_DistrictCode = this.m_CardNumber.Substring(0, 6);
				}
				return this.m_DistrictCode;
			}
		}

		public DateTime Birthday
		{
			get
			{
				if (this.m_Birthday == DateTime.MaxValue)
				{
					if (!DateTime.TryParse(m_CardNumber.Substring(6, 4) + "-" + m_CardNumber.Substring(10, 2) + "-" + m_CardNumber.Substring(12, 2), out this.m_Birthday))
					{
						this.m_Birthday = DateTime.MinValue;
					}
				}
				return this.m_Birthday;
			}
		}

		public int Sex
		{
			get
			{
				if (this.m_Sex == -1)
				{
					this.m_Sex = this.m_CardNumber[16] % 2;
				}
				return this.m_Sex;
			}
		}

		public bool ValidatedSuccess
		{
			get
			{
				if (this.m_Validated == -1)
				{
					this.m_Validated = (this.Birthday >= s_Range[0] && this.Birthday < s_Range[1]) && this.CodeCheck() ? 1 : 0;
				}
				return this.m_Validated.Equals(1);
			}
		}

		#endregion
		
		private bool CodeCheck()
		{
			int sum = 0;
			for(int i=0;i<17; i++)
			{
				sum += (this.m_CardNumber[i] - 48) * s_Wi[i];
			}
			return s_LastCode[sum % 11] == this.m_CardNumber[17];
		}
	}

	public interface IIDCard
	{
		string Province { get; }
		string City { get; }
		string District { get; }
		DateTime Birthday { get; }
		int Sex { get; }
		bool ValidatedSuccess { get; }
	}
	
	
}

