/*

FL Studio FLP file format
(version 11.x)



How it works

The FLP format is loosely based on MIDI and AIFF files. Apart from the header, it's just a succession of events. Once you understand how to process the file to retrieve these events, the only thing you'll need is the list of events available.

Please note that the format *does not* respect the AIFF standard completely, but it's similar.

DWORD is 4 bytes, WORD is 2 bytes, BYTE is 1 byte.

TEXT is a sequence of characters. Up to and including version 11, these were single-byte ansi characters. From version 12 on, these are 2-byte unicode characters. The exception is the FLP_Version event, which remains ansi (for compatibility).



Retrieving the events

While it looks like a MIDI file, it's not really a MIDI file.

First, you'll have to get & check the HEADER chunk, to be sure it's an FLP file.
The header is similar to the format of a MIDI file header:

DWORD	ChunkID	4 chars which are the letters 'FLhd' for 'FruityLoops header'
DWORD	Length		The length of this chunk, like in MIDI files. Atm it's 6, but could evolve.
WORD		Format		Set to 0 for full songs.
WORD		nChannels	The total number of channels (not really used).
WORD		BeatDiv		Pulses per quarter of the song.


Then you'll encounter the DATA chunk, which is in fact the last chunk, the one containing all the events.

DWORD	ChunkID	4 chars which are the letters 'FLdt' for 'FruityLoops data'
DWORD	Length		The length of  (the rest of) this chunk (i.e. WITHOUT these 2 DWORDS)


The whole data chunk is a succession of EVENTS, which I'm going to explain...

To retrieve an event, first you read a byte (the event ID). According to this byte, the size of the event data varies:
0..63	The data after this byte is a BYTE (signed or unsigned, depending on the ID).
64..127	The data after this byte is a WORD.
128..191	The data after this byte is a DWORD.
192..255	The data after this byte is a variable-length block of data (a text for example).

That makes 64 BYTE events, 64 WORD events, 64 DWORD events & 64 TEXT events. The purpose of this split is of course to keep the file size small.
So you get the event ID & then you read the number of bytes according to this ID. Whether you process the event or not isn't important. What is important is that you can jump correctly to the next event if you skip it.


For TEXT (variable-length) events, you still have to read the size of the event, which is coded in the next byte(s) a bit like in MIDI files (but not inverted). After the size is the actual data, which you can process or skip.
To get the size of the event, you've got to read bytes until the last one, which has bit 7 off (the purpose of this compression is to reduce the file size again).

Start with a DWORD Size = 0. You're going to reconstruct the size by getting packs of 7 bits:
1.	Get a byte.
2.	Add the first 7 bits of this byte to Size.
3.	Check bit 7 (the last bit) of this byte. If it's on, go back to 1. to process the next byte.

To resume, if Size < 128 then it will occupy only 1 byte, else if Size < 16384 it will occupy only 2 bytes & so on...


So globally, you open the file, check the header, point to the data chunk & retrieve / filter all the events. 



Some constants

header Format values
FLP_Format_None         		=-1;      // temporary
FLP_Format_Song          		=0;        // full project
FLP_Format_Score         		=0x10;  // score
FLP_Format_Auto          		=FLP_Format_Score+8;  // automation
FLP_Format_ChanState    		=0x20;     // channel
FLP_Format_PlugState    		=0x30;     // plugin
FLP_Format_PlugState_Gen	=0x31;
FLP_Format_PlugState_FX  	=0x32;
FLP_Format_MixerState    	=0x40;     // mixer track
FLP_Format_Patcher       		=0x50;     // special: tells to Patcherize (internal)

plugin flags
Plug_Visible     		=1;         // editor is visible or not
Plug_Disabled    		=2;         // obsolete
Plug_Detached    		=4;         // editor is detached
Plug_Maximized   	=8;         // editor is maximized
Plug_Generator   		=16;        // plugin is a generator (can be loaded into a channel)
Plug_SD          		=32;        // smart disable option is on
Plug_TP          		=64;        // threaded processing option is on
Plug_Demo        		=128;       // saved with a demo version
Plug_HideSettings	=1 << 8;   // editor is in compact mode
Plug_Captionized 	=1 << 9;   // editor is captionized
Plug_DX          		=1 << 16;  // indicates the plugin is a DirectX plugin (obsolete)
Plug_EditorSize  		=2 << 16;  // editor size is specified (obsolete)
Plug_EditorFlags 	=Plug_Visible | Plug_Detached | Plug_Maximized | Plug_HideSettings | Plug_Captionized;



The events list

Some events are obvious. Some others will have to be explained in details. But to understand how to process the file, you've got to know how the FLP is stored because the order of most of these events *IS* important.

For example, when storing a channel, an FLP_NewChan event is added. Then any next 'channel' event will affect this newly created channel.

To spare some space, some events are stored only if they differ from their default value.

//  BYTE sized (0..63)
FLP_Byte      		=0;
FLP_ChanEnabled	=0;
FLP_NoteOn    		=1;                // +pos
FLP_ChanVol   		=2;                // obsolete
FLP_ChanPan   		=3;                // obsolete
FLP_MIDIChan  		=4;
FLP_MIDINote  		=5;
FLP_MIDIPatch 		=6;
FLP_MIDIBank  		=7;
FLP_LoopActive		=9;
FLP_ShowInfo 		 =10;
FLP_Shuffle   		=11;
FLP_MainVol   		=12;               // obsolete
FLP_FitToSteps		=13;               // obsolete byte version
FLP_Pitchable 		=14;               // obsolete
FLP_Zipped    		=15;
FLP_Delay_Flags	=16;              // obsolete
FLP_TimeSig_Num 	=17;
FLP_TimeSig_Beat	=18;
FLP_UseLoopPoints	=19;
FLP_LoopType  		=20;
FLP_ChanType  		=21;
FLP_TargetFXTrack	=22;
FLP_PanVolTab 		=23;               // log vol & circular pan tables
FLP_nStepsShown	=24;              // obsolete
FLP_SSLength  		=25;               // +length
FLP_SSLoop    		=26;
FLP_FXProps   		=27;               // FlipY, ReverseStereo, etc
FLP_Registered		=28;               // reg version
FLP_APDC      		=29;
FLP_TruncateClipNotes	=30;
FLP_EEAutoMode	=31;

// WORD sized (63..127)
FLP_Word     		=64;
FLP_NewChan  		=FLP_Word;
FLP_NewPat   		=FLP_Word+1;        // +PatNum (word)
FLP_Tempo    		=FLP_Word+2;        // obsolete, replaced by FLP_FineTempo
FLP_CurrentPatNum	=FLP_Word+3;
FLP_PatData  		=FLP_Word+4;
FLP_FX       		=FLP_Word+5;
FLP_FXFlags  		=FLP_Word+6;
FLP_FXCut    		=FLP_Word+7;
FLP_DotVol   		=FLP_Word+8;
FLP_DotPan   		=FLP_Word+9;
FLP_FXPreamp 		=FLP_Word+10;
FLP_FXDecay  		=FLP_Word+11;
FLP_FXAttack 		=FLP_Word+12;
FLP_DotNote  		=FLP_Word+13;
FLP_DotPitch 		=FLP_Word+14;
FLP_DotMix   		=FLP_Word+15;
FLP_MainPitch		=FLP_Word+16;
FLP_RandChan 		=FLP_Word+17;       // obsolete
FLP_MixChan  		=FLP_Word+18;       // obsolete
FLP_FXRes    		=FLP_Word+19;
FLP_OldSongLoopPos	=FLP_Word+20;     // obsolete
FLP_FXStDel 		=FLP_Word+21;
FLP_FX3      		=FLP_Word+22;
FLP_DotFRes  		=FLP_Word+23;
FLP_DotFCut  		=FLP_Word+24;
FLP_ShiftTime		=FLP_Word+25;
FLP_LoopEndBar	=FLP_Word+26;
FLP_Dot      		=FLP_Word+27;
FLP_DotShift 		=FLP_Word+28;
FLP_Tempo_Fine	=FLP_Word+29;      // obsolete, replaced by FLP_FineTempo
FLP_LayerChan 		=FLP_Word+30;
FLP_FXIcon    		=FLP_Word+31;
FLP_DotRel    		=FLP_Word+32;
FLP_SwingMix  		=FLP_Word+33;

// DWORD sized (128..191)
FLP_Int      		=128;
FLP_PluginColor		=FLP_Int;
FLP_PLItem   		=FLP_Int+1;         // Pos (word) +PatNum (word) (obsolete)
FLP_Echo     		=FLP_Int+2;
FLP_FXSine   		=FLP_Int+3;
FLP_CutCutBy 		=FLP_Int+4;
FLP_WindowH  		=FLP_Int+5;
FLP_MiddleNote		=FLP_Int+7;
FLP_Reserved  		=FLP_Int+8;        // may contain an invalid version info
FLP_MainResCut	=FLP_Int+9;        // obsolete
FLP_DelayFRes		=FLP_Int+10;
FLP_Reverb   		=FLP_Int+11;
FLP_StretchTime		=FLP_Int+12;
FLP_SSNote   		=FLP_Int+13;        // SimSynth patch middle note (obsolete)
FLP_FineTune 		=FLP_Int+14;
FLP_SampleFlags	=FLP_Int+15;
FLP_LayerFlags		=FLP_Int+16;
FLP_ChanFilterNum	=FLP_Int+17;
FLP_CurrentFilterNum	=FLP_Int+18;
FLP_FXOutChanNum	=FLP_Int+19;     // FX track output channel
FLP_NewTimeMarker	=FLP_Int+20;    // + Time & Mode in higher bits
FLP_FXColor  		=FLP_Int+21;
FLP_PatColor 		=FLP_Int+22;
FLP_PatAutoMode	=FLP_Int+23;      // obsolete
FLP_SongLoopPos	=FLP_Int+24;
FLP_AUSmpRate	=FLP_Int+25;
FLP_FXInChanNum	=FLP_Int+26;      // FX track input channel
FLP_PluginIcon		=FLP_Int+27;
FLP_FineTempo		=FLP_Int+28;

// Variable size (192..255)
FLP_Undef    			=192;               // +Length (VarLengthInt)
FLP_Text     			=FLP_Undef;  // +Length (VarLengthInt) +Text (Null Term. AnsiString)
FLP_Text_ChanName  		=FLP_Text;    // obsolete
FLP_Text_PatName   		=FLP_Text+1;
FLP_Text_Title     		=FLP_Text+2;
FLP_Text_Comment   		=FLP_Text+3;
FLP_Text_SampleFileName	=FLP_Text+4;
FLP_Text_URL       		=FLP_Text+5;
FLP_Text_CommentRTF		=FLP_Text+6;  // comments in Rich Text format
FLP_Version        		=FLP_Text+7;
FLP_RegName        		=FLP_Text+8;  // since 1.3.9 the (scrambled) reg name is stored in the FLP
FLP_Text_DefPluginName	=FLP_Text+9;
//FLP_Text_CommentRTF_SC	=FLP_Text+10;  // new comments in Rich Text format (obsolete)
FLP_Text_ProjDataPath		=FLP_Text+10;
FLP_Text_PluginName		=FLP_Text+11; // plugin's name
FLP_Text_FXName    		=FLP_Text+12; // FX track name
FLP_Text_TimeMarker		=FLP_Text+13; // time marker name
FLP_Text_Genre     		=FLP_Text+14;
FLP_Text_Author    		=FLP_Text+15;
FLP_MIDICtrls      		=FLP_Text+16;
FLP_Delay          			=FLP_Text+17;
FLP_TS404Params    		=FLP_Text+18;
FLP_DelayLine      		=FLP_Text+19; // obsolete
FLP_NewPlugin      		=FLP_Text+20; // new VST or DirectX plugin
FLP_PluginParams   		=FLP_Text+21;
FLP_Reserved2      		=FLP_Text+22; // used once for testing
FLP_ChanParams     		=FLP_Text+23; // block of various channel params (can grow)
FLP_CtrlRecChan    		=FLP_Text+24; // automated controller events
FLP_PLSel          			=FLP_Text+25; // selection in playlist
FLP_Envelope       		=FLP_Text+26;
FLP_ChanLevels     		=FLP_Text+27; // pan, vol, pitch, filter, filter type
FLP_ChanFilter     		=FLP_Text+28; // cut, res, type (obsolete)
FLP_ChanPoly       		=FLP_Text+29; // max poly, poly slide, monophonic
FLP_NoteRecChan    		=FLP_Text+30; // automated note events
FLP_PatCtrlRecChan 		=FLP_Text+31; // automated ctrl events per pattern
FLP_PatNoteRecChan 		=FLP_Text+32; // automated note events per pattern
FLP_InitCtrlRecChan		=FLP_Text+33; // init values for automated events
FLP_RemoteCtrl_MIDI		=FLP_Text+34; // remote control entry (MIDI)
FLP_RemoteCtrl_Int 		=FLP_Text+35; // remote control entry (internal)
FLP_Tracking       		=FLP_Text+36; // vol/kb tracking
FLP_ChanOfsLevels  		=FLP_Text+37; // levels offset
FLP_Text_RemoteCtrlFormula	=FLP_Text+38; // remote control entry formula
FLP_Text_ChanFilter		=FLP_Text+39;
FLP_RegBlackList   		=FLP_Text+40; // black list of reg codes
FLP_PLRecChan      		=FLP_Text+41; // playlist
FLP_ChanAC         		=FLP_Text+42; // channel articulator
FLP_FXRouting      		=FLP_Text+43;
FLP_FXParams       		=FLP_Text+44;
FLP_ProjectTime    		=FLP_Text+45;
FLP_PLTrackInfo    		=FLP_Text+46;
FLP_Text_PLTrackName		=FLP_Text+47;



*/
using System;
using System.Collections.Generic;
using Monad.FLParser;

namespace flpzvoog
{
	class Program
	{
		static void Main(string[] args) {
			if (args.Length > 0) {
				testAll(args[0]);
			}
		}
		static void testAll(string filePath) {
			Project project1 = Project.Load(filePath, false);
			dumpProj(project1, filePath);
		}
		static string qu(string txt) {
			return "'" + txt + "'";
		}
		static void dumpPlugin(Plugin plugin) {
			if (plugin == null) {
				Console.WriteLine("					,'Plugin': null");
			} else {
				Console.WriteLine("					,'Plugin': { 'FileName': '" + plugin.FileName + "' ,'Flags': '" + plugin.Flags 
					+ "' ,'Guid': '" + plugin.Guid + "' ,'InfoKind': '" + plugin.InfoKind + "' ,'InputInfo': '" + plugin.InputInfo 
					+ "' ,'MidiInPort': '" + plugin.MidiInPort + "' ,'MidiOutPort': '" + plugin.MidiOutPort + "' ,'Name': '" + plugin.Name 
					+ "' ,'NumInputs': '" + plugin.NumInputs + "' ,'NumOutputs': '" + plugin.NumOutputs 
					+ "' ,'OutputInfo': '" + plugin.OutputInfo + "' ,'PitchBendRange': '" + plugin.PitchBendRange 
					+ "' ,'State': '" + plugin.State + "' ,'VendorName': '" + plugin.VendorName + "' ,'VstId': '" + plugin.VstId 
					+ "' ,'VstNumber': '" + plugin.VstNumber + "' }");
			}
		}
		static void dumpGenerator(GeneratorData gd) {
			Console.WriteLine("			,'generator': {");
			Console.WriteLine("				'ArpChord': '" + gd.ArpChord + "'");
			Console.WriteLine("				,'ArpDir': '" + gd.ArpDir + "'");
			Console.WriteLine("				,'ArpGate': '" + gd.ArpGate + "'");
			Console.WriteLine("				,'ArpRange': '" + gd.ArpRange + "'");
			Console.WriteLine("				,'ArpRepeat': '" + gd.ArpRepeat + "'");
			Console.WriteLine("				,'ArpSlide': '" + gd.ArpSlide + "'");
			Console.WriteLine("				,'ArpTime': '" + gd.ArpTime + "'");
			Console.WriteLine("				,'BaseNote': '" + gd.BaseNote + "'");
			Console.WriteLine("				,'GeneratorName': '" + gd.GeneratorName + "'");
			Console.WriteLine("				,'Insert': '" + gd.Insert + "'");
			Console.WriteLine("				,'LayerParent': '" + gd.LayerParent + "'");
			Console.WriteLine("				,'Panning': '" + gd.Panning + "'");
			dumpPlugin(gd.Plugin);
			Console.WriteLine("				,'PluginSettings': '" + gd.PluginSettings + "'");
			Console.WriteLine("				,'SampleAmp': '" + gd.SampleAmp + "'");
			Console.WriteLine("				,'SampleFileName': '" + gd.SampleFileName + "'");
			Console.WriteLine("				,'SampleReversed': '" + gd.SampleReversed + "'");
			Console.WriteLine("				,'SampleReverseStereo': '" + gd.SampleReverseStereo + "'");
			Console.WriteLine("				,'SampleUseLoopPoints': '" + gd.SampleUseLoopPoints + "'");
			Console.WriteLine("				,'Volume ': '" + gd.Volume + "'");
			Console.WriteLine("				}");
		}
		/*static void dumpKeyFrame(AutomationKeyframe kf, string zp) {
			Console.WriteLine("					" + zp + "{'Value':'" + kf.Value + "'"
					+ ", 'Position':'" + kf.Position + "'"
					+ ", 'Tension':'" + kf.Tension + "'}");
		}*/
		static void dumpAutomationData(AutomationData aut) {
			Console.WriteLine("			,'automation': {");
			if (aut.Channel == null) {
				Console.WriteLine("				'Channel': null");
			} else {
				Console.WriteLine("				'Channel': '" + aut.Channel.Id + "'");
			}
			Console.WriteLine("				,'InsertId': '" + aut.InsertId + "'");
			//Console.WriteLine("				,'Keyframes': [");
			string zp = "";
			string txtkf="";
			for (int kk = 0; kk < aut.Keyframes.Length; kk++) {
				//dumpKeyFrame(aut.Keyframes[kk], zp);
				txtkf= txtkf+ zp + "{'Value':'" + aut.Keyframes[kk].Value + "'"
					+ ", 'Position':'" + aut.Keyframes[kk].Position + "'"
					+ ", 'Tension':'" + aut.Keyframes[kk].Tension + "'}";
				zp = ", ";
			}
			//Console.WriteLine("					]");
			Console.WriteLine("				,'Keyframes': ["+txtkf+"]");
			Console.WriteLine("				,'Parameter': '" + aut.Parameter + "'");
			Console.WriteLine("				,'SlotId': '" + aut.SlotId + "'");
			Console.WriteLine("				,'VstParameter': '" + aut.VstParameter + "'");
			Console.WriteLine("				}");
		}
		static void dumpChannel(Channel singleChan, string dlmtr) {
			Console.WriteLine("		" + dlmtr + "{'Id': '" + singleChan.Id + "', 'Name': '" + singleChan.ChannelName + "' ,'Color':'" + singleChan.Color + "'");
			if (singleChan.Data.GetType() == typeof(GeneratorData)) {
				dumpGenerator((GeneratorData)singleChan.Data);
				Console.WriteLine("			,'automation': null");
			} else {
				if (singleChan.Data.GetType() == typeof(AutomationData)) {
					AutomationData aut = (AutomationData)singleChan.Data;
					Console.WriteLine("			,'generator': null");
					dumpAutomationData(aut);
				} else {
					Console.WriteLine("			,'generator': null, 'automation': null");
				}
			}
			Console.WriteLine("			}");
		}
		static void dumpSlot(InsertSlot insertSlot, string idlmtr) {
			Console.WriteLine("				" + idlmtr + "{");
			
			Console.WriteLine("					'DryWet':'" + insertSlot.DryWet + "'");
			
			dumpPlugin(insertSlot.Plugin);
			if (insertSlot.PluginSettings==null) {
				Console.WriteLine("					,'PluginSettings':null");
			} else { 
				Console.WriteLine("					,'PluginSettings':'" + insertSlot.PluginSettings.Length + "'");
			}
			//Console.Error.WriteLine("insertSlot " + insertSlot);
			Console.WriteLine("					,'State':'" + insertSlot.State + "'");
			Console.WriteLine("					,'Volume':'" + insertSlot.Volume + "'");
			Console.WriteLine("					}");
		}
		static void dumpInsert(Insert singleInsert, string dlmtr) {
			Console.WriteLine("		" + dlmtr + "{");
			Console.WriteLine("			Id: '" + singleInsert.Id + "'");
			Console.WriteLine("			,BandFreq : '" + singleInsert.BandFreq + "'");
			Console.WriteLine("			,BandLevel : '" + singleInsert.BandLevel + "'");
			Console.WriteLine("			,BandWidth : '" + singleInsert.BandWidth + "'");
			Console.WriteLine("			,'Color':'" + singleInsert.Color + "'");
			Console.WriteLine("			,Flags : '" + singleInsert.Flags + "'");
			Console.WriteLine("			,HighFreq : '" + singleInsert.HighFreq + "'");
			Console.WriteLine("			,HighLevel : '" + singleInsert.HighLevel + "'");
			Console.WriteLine("			,HighWidth : '" + singleInsert.HighWidth + "'");
			Console.WriteLine("			,LowFreq : '" + singleInsert.LowFreq + "'");
			Console.WriteLine("			,LowLevel: '" + singleInsert.LowLevel + "'");
			Console.WriteLine("			,LowWidth : '" + singleInsert.LowWidth + "'");
			Console.WriteLine("			,Name : '" + singleInsert.Name + "'");
			Console.WriteLine("			,Pan : '" + singleInsert.Pan + "'");
			string idlmtr = "";
			string rts = "";
			for (int rr = 0; rr < singleInsert.Routes.Length; rr++) {
				rts = rts + idlmtr + singleInsert.Routes[rr];
				idlmtr = ",";
			}
			Console.WriteLine("			,Routes: [" + rts + "]");
			idlmtr = "";
			rts = "";
			for (int rr = 0; rr < singleInsert.RouteVolumes.Length; rr++) {
				rts = rts + idlmtr + singleInsert.RouteVolumes[rr];
				idlmtr = ",";
			}
			Console.WriteLine("			,RouteVolumes: [" + rts + "]");
			Console.WriteLine("			,Slots: [");
			idlmtr = "";
			for (int rr = 0; rr < singleInsert.Slots.Length; rr++) {
				InsertSlot insertSlot = singleInsert.Slots[rr];
				dumpSlot(insertSlot, idlmtr);
				idlmtr = ",";
			}
			Console.WriteLine("				]");
			Console.WriteLine("			,StereoSep : '" + singleInsert.StereoSep + "'");
			Console.WriteLine("			,Volume : '" + singleInsert.Volume + "'");
			Console.WriteLine("			}");
		}
		static void dumpPlayListItem(IPlaylistItem itm, string itdel) {
			string txtline = "				" + itdel + "{";
			txtline = txtline + "'EndOffset': '" + itm.EndOffset + "'";
			txtline = txtline + ", 'Length': '" + itm.Length + "'";
			txtline = txtline + ", 'Position': '" + itm.Position + "'";
			txtline = txtline + ", 'StartOffset': '" + itm.StartOffset + "'";
			if (itm.GetType() == typeof(PatternPlaylistItem)) {
				PatternPlaylistItem pli = (PatternPlaylistItem)itm;
				txtline = txtline + ", 'pattern_id': '" + pli.Pattern.Id + "'";
				txtline = txtline + ", 'channel_id': null";
				txtline = txtline + ", 'muted': '" + pli.Muted + "'";
			} else {
				if (itm.GetType() == typeof(ChannelPlaylistItem)) {
					ChannelPlaylistItem chpli = (ChannelPlaylistItem)itm;
					txtline = txtline + ", 'pattern_id': null";
					txtline = txtline + ", 'channel_id': '" + chpli.Channel.Id + "'";
					txtline = txtline + ", 'muted': '" + chpli.Muted + "'";
				} else {
					txtline = txtline + ", 'pattern_id': null";
					txtline = txtline + ", 'channel_id': null";
					txtline = txtline + ", 'muted': null";
				}
			}
			txtline = txtline + "}";
			Console.WriteLine(txtline);
		}
		static void dumpTrack(Track track, string dlmtr, int tt) {
			Console.WriteLine("		" + dlmtr + "{");
			Console.WriteLine("			'Color':'" + track.Color + "'");
			Console.WriteLine("			,'Items':[");
			string itdel = "";
			for (int ii = 0; ii < track.Items.Count; ii++) {
				IPlaylistItem itm = track.Items[ii];
				dumpPlayListItem(itm, itdel);
				itdel = ",";
			}
			Console.WriteLine("				]");
			Console.WriteLine("			,'Name':'" + track.Name + "'");
			Console.WriteLine("			,'order':'" + tt + "'");
			Console.WriteLine("			}");
		}
		static void dumpPattern(Pattern pattern, string dlmtr) {
			Console.WriteLine("		" + dlmtr + "{");
			Console.WriteLine("			,'Id':'" + pattern.Id + "'");
			Console.WriteLine("			,'Name':'" + pattern.Name + "'");
			//Console.WriteLine("			,'Notes':[");
			string notesTxt="";
			string chanDlmtr = "";
			foreach (KeyValuePair<Channel, List<Note>> notesInChannel in pattern.Notes) {
				for (int i = 0; i < notesInChannel.Value.Count; i++) {
					Note note = notesInChannel.Value[i];
					notesTxt= notesTxt + chanDlmtr + " {'ChannelId': '" + notesInChannel.Key.Id + "', 'Note': {"
						+ "'FinePitch':'" + note.FinePitch + "'"
						+ ", 'Key':'" + note.Key + "'"
						+ ", 'Length':'" + note.Length + "'"
						+ ", 'Pan':'" + note.Pan + "'"
						+ ", 'Position':'" + note.Position + "'"
						+ ", 'Release':'" + note.Release + "'"
						+ ", 'Velocity':'" + note.Velocity + "'"
						+ "}}"
						;
				}
				chanDlmtr = ",";
			}
			Console.WriteLine("			,'Notes':["+ notesTxt + "]");
			Console.WriteLine("			}");
		}
		static void dumpProj(Project project, string filePath) {
			string dlmtr = "";
			Console.WriteLine("{");
			Console.WriteLine("	'dump': '1.0.1'");
			Console.WriteLine("	,'origin': '" + filePath + "'");
			Console.WriteLine("	,'Author': '" + project.Author + "'");
			List<Channel> chans = project.Channels;
			Console.WriteLine("	,'Channels': [");
			dlmtr = "";
			for (int ch = 0; ch < chans.Count; ch++) {
				Channel singleChan = chans[ch];
				dumpChannel(singleChan, dlmtr);
				dlmtr = ",";
			}
			Console.WriteLine("		]");
			Console.WriteLine("	,'Comment': '" + project.Comment.Replace('\n', ' ').Replace('\r', ' ') + "'");
			Console.WriteLine("	,'Genre': '" + project.Genre + "'");
			Console.WriteLine("	,'Inserts': [");
			dlmtr = "";
			for (int ch = 0; ch < project.Inserts.Length; ch++) {
				Insert singleInsert = project.Inserts[ch];
				if (singleInsert.Flags > 0) {
					//dumpInsert(singleInsert, dlmtr);
					dlmtr = ",";
				}
			}
			Console.WriteLine("		]");
			Console.WriteLine("	,'MainPitch': '" + project.MainPitch + "'");
			Console.WriteLine("	,'MainVolume': '" + project.MainVolume + "'");
			Console.WriteLine("	,'PlayTruncatedNotes': '" + project.PlayTruncatedNotes + "'");
			Console.WriteLine("	,'Ppq': '" + project.Ppq + "'");
			Console.WriteLine("	,'ProjectTitle': '" + project.ProjectTitle + "'");
			Console.WriteLine("	,'Tempo': '" + project.Tempo + "'");
			Console.WriteLine("	,'Tracks': [");
			dlmtr = "";
			for (int tt = 0; tt < project.Tracks.Length; tt++) {
				Track track = project.Tracks[tt];
				if (track.Items.Count > 0) {
					dumpTrack(track, dlmtr, tt);
					dlmtr = ",";
				}
			}
			Console.WriteLine("		]");
			Console.WriteLine("	,'Version': '" + project.Version + "'");
			Console.WriteLine("	,'VersionString': '" + project.VersionString + "'");
			Console.WriteLine("	,'Patterns': [");
			dlmtr = "";
			for (int tt = 0; tt < project.Patterns.Count; tt++) {
				Pattern pattern = project.Patterns[tt];
				dumpPattern(pattern, dlmtr);
				dlmtr = ",";
			}
			Console.WriteLine("}");
		}
	}
}
