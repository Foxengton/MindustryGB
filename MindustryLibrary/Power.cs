using Dapper;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace MindustryLibrary
{
	public class Power
	{
		#region//===== DATABASE =====//
		public static Power[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				return cnn.Query<Power>("SELECT * FROM Powers;", new DynamicParameters()).ToArray();
			}
		}
		public void Save()
		{
			if (Powers.Count(gen => gen.Id == Id) != 0)
			{
				Update();
				return;
			}

			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"INSERT INTO Powers VALUES(@Id, @GeneralId, @InputOutputId, @PowerUse, @PowerGeneration, @PowerCapacity);", this);
			}
		}
		public void Update()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"UPDATE Powers SET GeneralId = @GeneralId, InputOutputId = @InputOutputId, PowerUse = @PowerUse, PowerGeneration = @PowerGeneration, PowerCapacity = @PowerCapacity WHERE Id = @Id;", this);
			}
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"DELETE FROM Powers WHERE Id = @Id;", this);
			}
		}

		public string Id { get; set; }
		public string GeneralId { get; set; }
		public string InputOutputId { get; set; }
		public string PowerUse { get; set; }
		public string PowerGeneration { get; set; }
		public string PowerCapacity { get; set; }

		public static Power[] Powers => Load();

		public static string DBPath { get; set; }
		#endregion

		#region//===== OTHER FUNCTION =====//
		public static Power GetPower(string id) => Powers.Count(power => power.Id == id) != 0 ? Powers.First(power => power.Id == id) : null;
		public General GetGeneral => General.GetGeneral(GeneralId);
		public InputOutput GetInputOutput => InputOutput.GetInputOutput(InputOutputId);


		public static int Count => Powers.Count();
		public static string NextId => Count == 0 ? "0" : (Powers.Max(power => Convert.ToInt32(power.Id)) + 1).ToString();

		#endregion

		#region//===== OVERRIDES =====//
		public override string ToString()
		{
			string[] power = new string[3];

			if (PowerUse != null)
				power[0] = $"{PowerUse} use";
			if (PowerGeneration != null)
				power[1] = $"{PowerGeneration} generation";
			if (PowerCapacity != null)
				power[2] = $"{PowerCapacity} capacity";

			power = power.Where(pow => pow != null).ToArray();

			return $"{GetGeneral.Name}: {string.Join("/", power)}";
		}
		#endregion
	}
}