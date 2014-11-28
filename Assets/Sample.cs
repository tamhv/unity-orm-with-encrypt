using UnityEngine;
using System.Collections;

using UnityORM;
using System;

public class Sample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SqliteInit.InitSqlite();
		
		
		FieldLister lister = new FieldLister();
		
		UserData[] data = new UserData[2];
		
		data[0] = new UserData();
		data[0].ID = 1;
		data[0].Name = "Tarou";
		data[0].Hoge = "fuga";
		data[0].Age = 32;
		data[0].LastUpdated = new DateTime(2013,4,1);
		data[0].NestedClass.Fuga = "bbbb";
		data[0].NestedClass.Hoge = 23;
		
		data[1] = new UserData();
		data[1].ID = 2;
		data[1].Name = "Jirou";
		data[1].Hoge = "wahoo";
		data[1].Age = 11;
		data[1].AddressData = "aaaaa";
		data[1].LastUpdated = new DateTime(2013,5,1);
		
		Write(data[0].ToString());
		
		
		var info = lister.ListUp<UserData>();
		
		Write(info.ToString());
		
		var sqlMaker = new SQLMaker();
		string insert = sqlMaker.GenerateInsertSQL(info,data[0]);
		string update = sqlMaker.GenerateUpdateSQL(info,data[0]);
		
		Write("Insert = {0}",insert);
		Write("Update = {0}", update);
		
		DBMapper mapper = new DBMapper(SqliteInit.Evolution.Database);
		
		mapper.UpdateOrInsertAll(data);
		
		UserData[] fromDb = mapper.Read<UserData>("SELECT * FROM UserData;");
		Write(fromDb[0].ToString());
		Write(fromDb[1].ToString());
		
		
		JSONMapper jsonMapper = new JSONMapper();
		
		string json = jsonMapper.Write<UserData>(fromDb);
		UserData[] fromJson = jsonMapper.Read<UserData>(json);
		
		Write("Json = {0}", json);
		
		Write(fromJson[0].ToString());
		Write(fromJson[1].ToString());
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up, 5);
	}

	string text = "";

	void Write(string fmt, params object[] pars)
	{
		lock (text) {
			text += string.Format("\n{0}\t{1}\n", DateTime.Now, string.Format(fmt, pars));
		}
	}

	void OnGUI()
	{
		lock (text) {

			GUI.TextArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20), text);
		}
	}
}


public class UserData{
	public long ID;
	
	public string Name;
	
	public int Age;
	
	[Ignore]
	public string Hoge{get;set;}
	
	[MetaInfoAttirbute(NameInJSON = "address_data")]
	public string AddressData{get;set;}
	
	public DateTime LastUpdated;
	
	public Nested NestedClass = new Nested();
	
	public override string ToString ()
	{
		return "ID:" + ID + " Name:" + Name + " Hoge:" + Hoge + " Age:" + Age + " Address:" + AddressData +
			" LastUpdated:" + LastUpdated + " NestedClass:" + NestedClass;
	}
}
public class Nested{
	public int Hoge;
	public string Fuga;
	public override string ToString ()
	{
		return "Hoge : " + Hoge + " Fuga:" + Fuga;
	}
}