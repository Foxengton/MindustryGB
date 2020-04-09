using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace MindustryLibrary
{
	public class TypeItem
	{
		#region//===== DATABASE =====//
		public static TypeItem[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				return cnn.Query<TypeItem>("SELECT * FROM Types;", new DynamicParameters()).ToArray();
			}
		}
		public void Save()
		{
			if (Types.Count(type => type.Id == Id) != 0)
			{
				Update();
				return;
			}

			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"INSERT INTO Types VALUES(@Id, @Name, @Field);", this);
			}
		}
		public void Update()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"UPDATE Types SET Name = @Name, Field = @Field WHERE Id = @Id;", this);
			}
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"DELETE FROM Types WHERE Id = @Id;", this);
			}
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Field { get; set; }

		public static TypeItem[] Types => Load();

		public static string DBPath { get; set; }
		#endregion

		#region//===== OTHER FUNCTION =====//
		public static TypeItem GetType(string id) => Types.Count(type => type.Id == id) != 0 ? Types.First(type => type.Id == id) : null;
		public static TypeItem[] GetKind(string kind) => Types.Count(type => type.Field == kind) != 0 ? Types.Where(type => type.Field == kind).ToArray() : null;

		public static int Count => Types.Count();
		public static string NextId => Count == 0 ? "0" : (Types.Max(power => Convert.ToInt32(power.Id)) + 1).ToString();
		#endregion

		#region//===== OVERRIDES =====//
		public override string ToString()
		{
			return $"{Name}";
		}
		#endregion
	}
}
