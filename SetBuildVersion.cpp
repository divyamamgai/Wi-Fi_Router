#include<iostream>
#include<conio.h>
#include<string.h> 
#include<fstream>    
#include<sstream>
#include <cstdlib>
#include<stdio.h>
#include <windows.h>
using namespace std;
string CurrentDirectory() {
    char Buffer[MAX_PATH];
    GetModuleFileName( NULL, Buffer, MAX_PATH );
    string::size_type Position = string(Buffer).find_last_of( "\\/" );
    return string(Buffer).substr(0,Position);
}
#define MAXSWITCHES 11
#define MAXSUBSWITCHES 5
#define MAXSWITCHINPUT 6
string Switches[MAXSWITCHES][MAXSUBSWITCHES] = {
    {"-h","-help","NULL","NULL","-NULL"}, // 1
    {"-d","-default","NULL","NULL","NULL"}, // 2
    {"-f","-force","NULL","NULL","NULL"}, // 3
    {"-a","-auto","NULL","NULL","NULL"}, // 4
    {"-as","-autos","-autosub","NULL","NULL"}, // 5
    {"-am","-autom","-automajor","NULL","NULL"}, // 6
    {"-fmv","-forcemv","-forcemaxv","-forcemaxversion","NULL"}, // 7
    {"-fmsv","-forcemsv","-forcemaxsv","-forcemaxsubv","-forcemaxsubversion"}, // 8
    {"-n","-new","NULL","NULL","-NULL"}, // 9
    {"-ds","-defaultsetup","NULL","NULL","NULL"}, // 10
    {"-fs","-forcesetup","NULL","NULL","NULL"} // 11
};
int SwitchFlags[MAXSWITCHES] = {1,1,1,1,1,1,1,1,1,1,1};
int CurrentVersion[3] = {1,0,0};
int MaxVersion = 9;
int MaxSubVersion = 9;
string AssemblyInfoVbFilePath = "";
string SetupAipFilePath = "";
void _CMD(const char* Command) {
	string CommandString = "";
	CommandString+=Command;
	CommandString+=" > nul";
	FILE* Pipe = _popen(CommandString.c_str(), "r");
	if (!Pipe) cout<<"ERROR";
	_pclose(Pipe);
}
int GetSwitchIndex(char* Switch){
    for(int i=0;i<MAXSWITCHES;i++){
        for(int j=0;j<MAXSUBSWITCHES;j++){
            if(strcmpi(Switches[i][j].c_str(),(const char*)Switch)==0){
                return i+1;
            }
        }
    }
    return 0;
}
void ToggleSwitch(int SwitchID){
    if(SwitchFlags[SwitchID-1]==0) SwitchFlags[SwitchID-1] = 1;
    else if(SwitchFlags[SwitchID-1]==1) SwitchFlags[SwitchID-1] = 0;
}
void SetSwitch(int SwitchID,int State){
	SwitchFlags[SwitchID-1] = State;
}
int GetSwitchState(int SwitchID){
    return SwitchFlags[SwitchID-1];
}
int CheckFileType(string FilePath,int FileType = 0){
	int FileExtensionPos = 0;
	for(int i=0;i<strlen(FilePath.c_str());i++){
		if(FilePath[i]=='.'){
			FileExtensionPos = i;
			break;
		}
	}
	string Ext = "";
	if(FileType==0) Ext = ".vb";
	else Ext = ".aip";
	if(strcmp(FilePath.substr(FileExtensionPos,strlen(Ext.c_str())).c_str(),Ext.c_str())==0){
		return 1;
	}else{
		return 0;
	}
}
string _GetVersion(){
	stringstream VersionStringStream;
	VersionStringStream<<CurrentVersion[0]<<"."<<CurrentVersion[1]<<"."<<CurrentVersion[2];
	return VersionStringStream.str();
}
int InitCurrentVersion(string FilePath){
	string VersionString = "<Assembly: AssemblyVersion(\"";
	string VersionValidatorString = "";
	fstream VersionFile(FilePath.c_str(),ios::in);
	if(VersionFile.is_open()){
		if(CheckFileType(FilePath,0)){
			string InputLine;
			while(getline(VersionFile,InputLine)){
				size_t CommentPosition = InputLine.find("'");
				size_t TagOpenPosition = InputLine.find("<");
				if(CommentPosition!=string::npos){
					if(TagOpenPosition!=string::npos){
						if(TagOpenPosition<CommentPosition){
							size_t AssemblyVersionFound = InputLine.find(VersionString);
							if(AssemblyVersionFound!=string::npos){
								VersionString = InputLine.substr(AssemblyVersionFound+strlen(VersionString.c_str()),strlen(InputLine.c_str())-AssemblyVersionFound);
								size_t NextQuotePos = VersionString.find("\")>");
								if(NextQuotePos!=string::npos){
									VersionString = VersionString.substr(0,NextQuotePos);
									stringstream VersionStringStream(VersionString);
									string Version;
									int i=0;
									while(getline(VersionStringStream,Version,'.')){
										CurrentVersion[i] = atoi(Version.c_str());
										i++;
									}
									break;
								}else{
									return 0;
								}
							}
						}else{
							continue;
						}
					}else{
						continue;
					}
				}else{
					size_t AssemblyVersionFound = InputLine.find(VersionString);
					if(AssemblyVersionFound!=string::npos){
						VersionString = InputLine.substr(AssemblyVersionFound+strlen(VersionString.c_str()),strlen(InputLine.c_str())-AssemblyVersionFound);
						size_t NextQuotePos = VersionString.find("\")>");
						if(NextQuotePos!=string::npos){
							VersionString = VersionString.substr(0,NextQuotePos);
							stringstream VersionStringStream(VersionString);
							string Version;
							int i=0;
							while(getline(VersionStringStream,Version,'.')){
								CurrentVersion[i] = atoi(Version.c_str());
								i++;
							}
							break;
						}else{
							return 0;
						}
					}
				}
			}
		}else{
			cout<<"\n [Error::Provided file is not of correct type.]";
			return 0;
		}
	}else{
		cout<<"\n [Error::Provided file is inaccessible.]";
		return 0;
	}
	VersionFile.close();
	return 1;
}
int SetBuildVersionSetup(){
	string FilePath = SetupAipFilePath;
	string VersionString = "    <ROW Property=\"ProductVersion\" Value=\"";
	string VersionValidatorString = "";
	fstream VersionFile(FilePath.c_str(),ios::in);
	string FileWritePath = FilePath;
	FileWritePath+=".temp";
	fstream VersionFileWrite(FileWritePath.c_str(),ios::out);
	if(VersionFile.is_open()){
		if(CheckFileType(FilePath,1)){
			string InputLine;
			while(getline(VersionFile,InputLine)){
				size_t AssemblyVersionFound = InputLine.find(VersionString);
				if(AssemblyVersionFound!=string::npos){
					VersionFileWrite<<VersionString<<_GetVersion()<<"\" Type=\"32\"/>";
				}else{
					VersionFileWrite<<InputLine;					
				}
				VersionFileWrite<<"\n";
			}
		}else{
			cout<<"\n [Error::Provided file is not of correct type.]";
			return 0;
		}
	}else{
		cout<<"\n [Error::Provided file is inaccessible.]";
		return 0;
	}
	VersionFile.close(); 
	VersionFileWrite.close();
	if(remove(FilePath.c_str())==0){
		if(rename(FileWritePath.c_str(),FilePath.c_str())==0){
			cout<<" Done!\n Do not forget to create new Product ID Code for the setup!";
			return 1;
		}else{
			cout<<"\n [Error::The file cannot be renamed or is inaccessible.]";
			return 0;
		}
	}else{
		cout<<"\n [Error::Provided file is inaccessible.]";
		return 0;
	}
}
int SetBuildVersion(string FilePath){
	string VersionString = "<Assembly: AssemblyVersion(\"";
	string FileVersionString = "<Assembly: AssemblyFileVersion(\"";
	string VersionValidatorString = "";
	fstream VersionFile(FilePath.c_str(),ios::in);
	string FileWritePath = FilePath;
	FileWritePath+=".temp";
	fstream VersionFileWrite(FileWritePath.c_str(),ios::out);
	if(VersionFile.is_open()){
		if(CheckFileType(FilePath,0)){
			string InputLine;
			while(getline(VersionFile,InputLine)){
				size_t CommentPosition = InputLine.find("'");
				size_t TagOpenPosition = InputLine.find("<");
				if(CommentPosition!=string::npos){
					if(TagOpenPosition!=string::npos){
						if(TagOpenPosition<CommentPosition){
							size_t AssemblyVersionFound = InputLine.find(VersionString);
							size_t AssemblyFileVersionFound = InputLine.find(FileVersionString);
							if(AssemblyVersionFound!=string::npos){
								VersionFileWrite<<VersionString<<_GetVersion()<<"\")>";
							}else if(AssemblyFileVersionFound!=string::npos){
								VersionFileWrite<<FileVersionString<<_GetVersion()<<"\")>";
							}
						}else{
							VersionFileWrite<<InputLine;
						}
					}else{
						VersionFileWrite<<InputLine;
					}
				}else{
					size_t AssemblyVersionFound = InputLine.find(VersionString);
					size_t AssemblyFileVersionFound = InputLine.find(FileVersionString);
					if(AssemblyVersionFound!=string::npos){
						VersionFileWrite<<VersionString<<_GetVersion()<<"\")>";
					}else if(AssemblyFileVersionFound!=string::npos){
						VersionFileWrite<<FileVersionString<<_GetVersion()<<"\")>";
					}else{
						VersionFileWrite<<InputLine;					
					}
				}
				VersionFileWrite<<"\n";
			}
		}else{
			cout<<"\n [Error::Provided file is not of correct type.]";
			return 0;
		}
	}else{
		cout<<"\n [Error::Provided file is inaccessible.]";
		return 0;
	}
	VersionFile.close(); 
	VersionFileWrite.close();
	if(remove(FilePath.c_str())==0){
		if(rename(FileWritePath.c_str(),FilePath.c_str())==0){
			SetBuildVersionSetup();
		}else{
			cout<<"\n [Error::The file cannot be renamed or is inaccessible.]";
			return 0;
		}
	}else{
		cout<<"\n [Error::Provided file is inaccessible.]";
		return 0;
	}
}
string* Arguments;
void ClearArguments(){
    delete[] Arguments;
}
bool StringDigitOnly(const string &InputString)
{
    return InputString.find_first_not_of("0123456789") == string::npos;
}
bool StringVersionValidator(string InputString){
	int VersionCount = 0;
	stringstream InputStringStream(InputString);
	string InputStringSplit;
	while(getline(InputStringStream,InputStringSplit,'.')){
		if(VersionCount>2){
			return false;
		}else{
			if(!StringDigitOnly(InputStringSplit)){
				return false;
			}
		}
		VersionCount++;
	}
	return true;
}
void StringVersionSet(string InputString){
	int VersionCount = 0;
	stringstream InputStringStream(InputString);
	string InputStringSplit;
	while(getline(InputStringStream,InputStringSplit,'.')){
		CurrentVersion[VersionCount] = atoi(InputStringSplit.c_str());
		VersionCount++;
	}
}
int main(int argc,char *argv[],char* envp[]){
	AssemblyInfoVbFilePath = CurrentDirectory();
	AssemblyInfoVbFilePath+= "\\Wi-Fi Router\\Wi-Fi Router\\My Project\\AssemblyInfo.vb";
	SetupAipFilePath = CurrentDirectory();
	SetupAipFilePath+= "\\Wi-Fi Router.aip";
    atexit(ClearArguments);
    int SwitchCount = 0;
    for(int i=1;i<argc;i++){
        if(argv[i][0]=='-') SwitchCount++;
    }
    int ArgumentCount = 0;
    Arguments = new string[argc-SwitchCount];
    for(int i=1;i<argc;i++){
        if(argv[i][0]!='-'){
            Arguments[ArgumentCount] = (const char*)argv[i];
            ArgumentCount++; 
        }
    }
    int CurrentArgument = 0;
    if(GetSwitchIndex(argv[1])==1){
        Help:
        cout<<"\n Help for "<<argv[0]<<":"
            <<"\n  Available switches are:"
            <<"\n   [-h][-help]"
            <<"\n   "<<char(192)<<char(26)<<" For Help"
            <<"\n   [-d][-default]"
            <<"\n   "<<char(192)<<char(26)<<" To take AssemblyInfo.vb file path as default."
            <<"\n   [-f][-force]"
            <<"\n   "<<char(192)<<char(26)<<" To force to take AssemblyInfo.vb file path as provided."
            <<"\n   [-a][-auto]"
            <<"\n   "<<char(192)<<char(26)<<" To automatically generate next version. {1.0.x}"
            <<"\n   [-as][-autos][-autosub]"
            <<"\n   "<<char(192)<<char(26)<<" For automatically generate next sub-version. {1.x.0}"
            <<"\n   [-am][-autom][-automajor]"
            <<"\n   "<<char(192)<<char(26)<<" For automatically generate next major-version. {x.0.0}"
            <<"\n   [-fmv][-forcemv][-forcemaxv][-forcemaxversion]"
            <<"\n   "<<char(192)<<char(26)<<" To force to take max version value as provided."
            <<"\n   [-fmsv][-forcemsv][-forcemaxsv][-forcemaxsubv][-forcemaxsubversion]"
            <<"\n   "<<char(192)<<char(26)<<" To force to take max sub-version value as provided."
            <<"\n  How to use:"
            <<"\n   "<<argv[0]<<" [Switches] [FilePath{If Required}] [Version{If Required}]"
            <<"\n";
            return 0;
	}
    if(SwitchCount<=MAXSWITCHINPUT){
		int k = 1;
        for(int i=1;i<=SwitchCount;i++){
			if(argv[k][0]!='-'){
				i--;
				k++;
				continue;
			}
            switch(GetSwitchIndex(argv[k])){
                case 1:
                    goto Help;
                    break;
                case 2:
                    if(GetSwitchState(2)){
                        AssemblyInfoVbFilePath = CurrentDirectory();
						AssemblyInfoVbFilePath+= "\\Wi-Fi Router\\Wi-Fi Router\\My Project\\AssemblyInfo.vb";
                        cout<<"\n Using default path for AssemblyInfo.vb file.";
                        if(InitCurrentVersion(AssemblyInfoVbFilePath)){
							cout<<"\n Current Version is : "<<_GetVersion();
						}else{
							return 0;
						}
                        ToggleSwitch(3);
                    }else{
                        cout<<"\n [Warning::Force switch has overridden the Default switch. Please place Default switch before Force.]";
                    }
                    break;
                case 3:
                    if(GetSwitchState(3)){
                        if((ArgumentCount>0)&&(ArgumentCount>=CurrentArgument)){
                        	AssemblyInfoVbFilePath = Arguments[CurrentArgument];
                        	cout<<"\n Using ["<<AssemblyInfoVbFilePath<<"] as path for AssemblyInfo.vb file.";
                        	if(InitCurrentVersion(AssemblyInfoVbFilePath)){
								cout<<"\n Current Version is : "<<_GetVersion();
							}else{
								return 0;
							}
                        	CurrentArgument++;
                        	if(CurrentArgument>ArgumentCount) CurrentArgument = ArgumentCount;
                        }else{
							cout<<"\n [Error::No argument was passed to Force switch.]";
							return 0;
						}
                    }else{
                        cout<<"\n [Warning::Default switch has overridden the Force switch. Please place Force switch before Default.]";
                    }
                    break;
                case 4:
                    if(GetSwitchState(4)){
						ToggleSwitch(7);
						cout<<"\n Auto-incrementing version.";
						CurrentVersion[2]++;
						if(CurrentVersion[2]>MaxVersion){
							cout<<"\n [Warning::Incremented version was greater than the max version. Please use Force Max Version switch to increase the limit.]";
							CurrentVersion[2]--;
						}else{
							cout<<" [Incremented version : "<<_GetVersion()<<"]";
							SetSwitch(9,0);
						}
					}
                    break;
                case 5:
                    if(GetSwitchState(5)){
						ToggleSwitch(8);
						cout<<"\n Auto-incrementing sub-version."; 
						CurrentVersion[1]++;
						if(CurrentVersion[1]>MaxSubVersion){
							cout<<"\n [Warning::Incremented sub-version was greater than the max sub-version. Please use Force Max Sub-Version switch to increase the limit.]";
							CurrentVersion[1]--;
						}else{
							cout<<" [Incremented version : "<<_GetVersion()<<"]";
							SetSwitch(9,0);
						}
					}
                    break;
                case 6:
                    if(GetSwitchState(6)){
						cout<<"\n Auto-incrementing major-version.";
						CurrentVersion[0]++;
						cout<<" [Incremented version : "<<_GetVersion()<<"]";
						SetSwitch(9,0);
					}
                    break;
                case 7:
                    if(GetSwitchState(7)){
                        if((ArgumentCount>0)&&(ArgumentCount>=CurrentArgument)){
							if(StringDigitOnly(Arguments[CurrentArgument])){						
								cout<<"\n Forcing to take max version value as "<<Arguments[CurrentArgument]<<".";
								MaxVersion = atoi(Arguments[CurrentArgument].c_str());
							}else{
								cout<<"\n [Error::Argument passed to Force Max Version switch is not an integer]";
								return 0;							
							}
                        	CurrentArgument++;
                        	if(CurrentArgument>ArgumentCount) CurrentArgument = ArgumentCount;
                        }else{
							cout<<"\n [Error::No argument was passed to Force Max Version switch.]";
							return 0;
						}
					}else{
                        cout<<"\n [Warning::Auto Version switch has overridden the Force Max Version switch. Please place Force Max Version switch before Auto Version.]";
                    }
                    break;
                case 8:
                    if(GetSwitchState(8)){
                        if((ArgumentCount>0)&&(ArgumentCount>=CurrentArgument)){
							if(StringDigitOnly(Arguments[CurrentArgument])){						
								cout<<"\n Forcing to take max sub-version value as "<<Arguments[CurrentArgument]<<".";
								MaxSubVersion = atoi(Arguments[CurrentArgument].c_str());
							}else{
								cout<<"\n [Error::Argument passed to Force Max Sub-Version switch is not an integer]";
								return 0;							
							}
                        	CurrentArgument++;
                        	if(CurrentArgument>ArgumentCount) CurrentArgument = ArgumentCount;
                        }else{
							cout<<"\n [Error::No argument was passed to Force Max Sub-Version switch.]";
							return 0;
						}
					}else{
                        cout<<"\n [Warning::Auto Sub-Version switch has overridden the Force Max Sub-Version switch. Please place Force Max Sub-Version switch before Auto Sub-Version.]";
                    }
                    break;
                case 9:
                    if(GetSwitchState(9)){
                        if((ArgumentCount>0)&&(ArgumentCount>=CurrentArgument)){
							ToggleSwitch(4);   
							ToggleSwitch(5);
							ToggleSwitch(6);
							if(StringVersionValidator(Arguments[CurrentArgument])){
								StringVersionSet(Arguments[CurrentArgument]);
								cout<<"\n New Version is : "<<_GetVersion();
							}else{
								cout<<"\n [Error::Argument passed to New Version switch is not a valid version format.]";
								return 0;							
							}
                        	CurrentArgument++;
                        	if(CurrentArgument>ArgumentCount) CurrentArgument = ArgumentCount;
                        }else{
							cout<<"\n [Error::No argument was passed to New Version switch.]";
							return 0;
						}
					}else{
                        cout<<"\n [Warning::Auto Increment switch(es) has overridden the New Version switch. Please place New Version switch before Auto Increment Switch(es).]";
                    }
					break;
            }
            k++;
        }
    }
    cout<<"\n Finalizing Version Setting.";
    SetBuildVersion(AssemblyInfoVbFilePath);
    cout<<"\n";
    return 1;
}
