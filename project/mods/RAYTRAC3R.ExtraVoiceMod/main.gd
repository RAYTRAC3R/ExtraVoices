extends Node


onready var TackleBox := $"/root/TackleBox"

var config:Dictionary = {
  "Options": "Base Options are: NewVoice, OldVoice, Minty, Dhama",
  "VoiceOption": "NewVoice",
  "ModlessVoiceOption": "NewVoice"
}


#code kinda just stolen from base game
func voice_bank():
	Globals.voice_bank.clear()
	
	print("Creating Voice Bank...")
	var resource_count = 0
	var files = []
	var subdirectories = []
	var dir = Directory.new()
	var path = "res://Sounds/Voice/"
	
	if dir.open(path) != OK:
		print("Error loading voice directory.")
		return 
	
	dir.list_dir_begin(true, true)
	while true:
		var file = dir.get_next()
		if file == "":
			break
		elif dir.current_is_dir():
			subdirectories.append(file)
			print("Directory found: ", file)
	
	for directory in subdirectories:
		Globals.voice_bank[directory] = {}
		
		if dir.open(path + directory) != OK:
			print("Error loading voice subdirectory ", directory)
			break
		
		
		dir.list_dir_begin(true, true)
		while true:
			var file = dir.get_next()
			
			
			if file == "":
				
				break
			elif file.ends_with(".ogg.import"):
				
				var f = File.new()
				f.open(path + directory + "/" + file, File.READ)
				
				var read = f.get_as_text()
				var final_path = ""
				
				for line in read.split("\n"):
					if line.begins_with("path="):
						var l = line.split("=")[1]
						final_path = l.replace("\"", "")
				
				
				var end = load(final_path)
				Globals.voice_bank[directory][file.replace(".ogg.import", "")] = end
				resource_count += 1
	
	dir.list_dir_end()
	
func _ready():
	TackleBox.connect("mod_config_updated", self, "_update_config")
	
	_init_config(TackleBox.get_mod_config("RAYTRAC3R.ExtraVoiceMod"));
	voice_bank()
	
#code kinda just stolen from VeryUnlethalCoalition's YAAM
func _init_config(conf:Dictionary):
	if config.size() != conf.size():
		for key in config.keys():
			if !conf.has(key):
				conf[key] = config[key]
	config = conf
	print(conf)

func _update_config(mod_id, config):
	if mod_id == "RAYTRAC3R.ExtraVoiceMod":
		self.config = config
		PlayerData.player_voicebank = self.config["VoiceOption"]
		PlayerData.peer_voicebank = self.config["ModlessVoiceOption"]
