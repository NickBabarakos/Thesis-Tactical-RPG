using Godot;
using System;

public partial class character : Node
{
	private string _name;
	private int _hp;
	private int _atk;
	private int _def;
	private int _spAtk;
	private int _spDef;
	private int _spd;
	private int _vul;
	private int _res;
	private int _cHp;
	//public move moveInstance;

	public string Name{
		get {return _name;}
		set {_name = value;}}

	public int HP{
		get {return _hp;}
		set {_hp = value;}}
	public int CHP{
		get { return _cHp;}
		set {_cHp = value;}}
	
	public int ATK{
		get {return _atk;}
		set {_atk = value;}}
	
	public int DEF{
		get {return _def;}
		set {_def = value;}}
	
	public int SP_ATK{
		get {return _spAtk;}
		set {_spAtk = value;}}
	
	public int SP_DEF{
		get {return _spDef;}
		set {_spDef = value;}}
	
	public int SPD{
		get { return _spd;}
		set {_spd = value;}}
	
	public int VUL{
		get { return _vul;}
		set { _vul = value;}}
	
	public int RES{
		get { return _res;}
		set { _res = value;}}



	public character(string name, int hp, int chp, int atk, int def, int spAtk, int spDef, int spd, int vul, int res){
		Name = name;
		HP = hp;
		CHP = chp;
		ATK = atk;
		DEF = def;
		SP_ATK = spAtk;
		SP_DEF = spDef;
		SPD = spd;
		VUL = vul;
		RES = res;
	}

}
