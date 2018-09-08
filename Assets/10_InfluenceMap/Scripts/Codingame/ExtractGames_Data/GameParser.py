import glob
from nt import chdir
import re
import pandas as pd
import os
import shutil


pathname = r"E:\Unity Projects\InfluenceMap_Code_Royale_Codingame\Assets\10_InfluenceMap\Scripts\Codingame\ExtractGames_Data"
chdir(pathname)

outputLines=[]
for file in glob.glob("*.html"):
    file_name_noextension = os.path.splitext(file)[0]
    
    if(os.path.exists(file_name_noextension+".csv") is True):
        continue
    
    file_content = open(file, "r").read()
    print (file)
    outputLine1 = re.findall('trustHtml"><div class="outputLine">\s?.+?\s?\</div><div class="outputLine">\s?(.+?)\s?\</div', file_content)[0]
    
    outputFirstAndLastLines = re.findall('trustHtml">(?:<div class="outputLine">(\s?.+?\s?)\<\/div>)(?:<div class="outputLine">\s?.+?\s?\<\/div>)*<div class="outputLine">\s?(.+?)\s?\<\/div><\/pre><!-- end ngIf: ::frame.stderr -->', file_content)
    # outputLine1_ = re.findall('\|</div><div class="outputLine">\s?(.+?)\s?\</div', file_content)
    
    # print (outputLine1[0])
    # print ( outputLine1)

    # print ( outputFirstAndLastLines[0][0])
    # print ( outputFirstAndLastLines[0][0])
    # print ( outputFirstAndLastLines[4][1])

    # print ( outputLineLast ) #all game states and pulzella

    f = open(file_name_noextension+".csv","w+")
    
    f.write(outputLine1+'\n')   #Game info

    for line in outputFirstAndLastLines:
        f.write(line[0]+'\n')
        f.write(line[1]+'\n')

    f.close()
    print("Created file "+f.name)
    os.remove(file)
    print("Deleted file "+file)
    # os.remove(file_name_noextension+"_files")
    shutil.rmtree(file_name_noextension+"_files")
    print("Removed Folder "+file_name_noextension+"_files")
    


    
    


