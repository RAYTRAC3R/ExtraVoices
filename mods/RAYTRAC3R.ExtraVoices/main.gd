extends Node


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	Globals.voice_bank.clear()
	
	print("Creating Voice Bank...")
	var resource_count = 0
	var files = []
	var subdirectories = []
	var dir = Directory.new()
	var path = "res://Sounds/Voice/"
	
	DebugScreen._add_line("Voice Direct Loading")
	
	if dir.open(path) != OK:
		DebugScreen._add_line("Voice Direct Error")
		print("Error loading voice directory.")
		return 
	
	DebugScreen._add_line("Voice Direct Found")
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
			DebugScreen._add_line("Voice Subdirect error, " + str(directory))
			print("Error loading voice subdirectory ", directory)
			break
		
		DebugScreen._add_line("Voice Subdirect success, " + str(directory))
		
		dir.list_dir_begin(true, true)
		while true:
			var file = dir.get_next()
			
			DebugScreen._add_line("Reading file " + str(file))
			
			if file == "":
				DebugScreen._add_line("Voice files end here")
				
				break
			elif file.ends_with(".ogg.import"):
				DebugScreen._add_line("OGG Voice access: " + str(file))
				
				var f = File.new()
				f.open(path + directory + "/" + file, File.READ)
				
				var read = f.get_as_text()
				var final_path = ""
				
				for line in read.split("\n"):
					if line.begins_with("path="):
						var l = line.split("=")[1]
						final_path = l.replace("\"", "")
				
				DebugScreen._add_line(str(final_path))
				
				var end = load(final_path)
				Globals.voice_bank[directory][file.replace(".ogg.import", "")] = end
				resource_count += 1
	
	dir.list_dir_end()
	DebugScreen._add_line("Voice Direct: " + str(Globals.voice_bank))


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
