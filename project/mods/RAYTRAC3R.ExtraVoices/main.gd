extends Node


var PlayerAPI


# Called when the node enters the scene tree for the first time.
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
	
func get_player_voice():
	if Network.PLAYING_OFFLINE:
		return
		
func readPackets():
	if Network.PLAYING_OFFLINE: return
	
	var PACKET_SIZE = Steam.getAvailableP2PPacketSize(4)
	if PACKET_SIZE > 0:
		var PACKET = Steam.readP2PPacket(PACKET_SIZE, 4)
		
		if PACKET.empty():
			print("Error! Empty Packet!")
		
		var data = bytes2var(PACKET.data.decompress_dynamic( - 1, File.COMPRESSION_GZIP))
		
		#PlayerData._send_notification("[RECEIVE NET] from: " + str(data.steamid) + " ... " + str(data))

		emit_signal("tourney_net", data.steamid, data)


func _ready():
	PlayerAPI = get_node_or_null("/root/BlueberryWolfiAPIs/PlayerAPI")
	PlayerAPI.connect("_player_added", self, "init_player")
	PlayerAPI.connect("_ingame", self, "init_player")
	voice_bank()

func init_player(player: Actor):
	# example:
	print(PlayerAPI.get_player_name(player))
	
	
func _process(delta):
	if Network.STEAM_LOBBY_ID > 0:
		readPackets()
