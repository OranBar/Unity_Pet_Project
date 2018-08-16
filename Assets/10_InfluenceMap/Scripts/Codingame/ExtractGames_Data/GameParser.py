import glob
from nt import chdir
import re
import pandas as pd
import os


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
    # outputLineLast = re.findall('trustHtml">(?:<div class="outputLine">\s?.+?\s?\<\/div>)*<div class="outputLine">\s?(.+?)\s?\<\/div><\/pre><!-- end ngIf: ::frame.stderr -->', file_content)
    # outputLine1_ = re.findall('\|</div><div class="outputLine">\s?(.+?)\s?\</div', file_content)
    
 
    # print (outputLine0[0])  #game info
    # print (outputLine1[0])
    # print ( outputLine1)

    # print ( outputFirstAndLastLines[0][0])
    # print ( outputFirstAndLastLines[0][0])
    # print ( outputFirstAndLastLines[4][1])

    # outputLines.append (outputLine0[0])  #game info
    # outputLines.append ( outputLineLast ) #Each turn is a two lines: one is all game states and pulzella separated by '-', other is chosen tile coordinates

    # outputLines.append (outputLine0[0])  #game info
    # print ( outputLineLast ) #all game states and pulzella

    f = open(file_name_noextension+".csv","w+")
    
    f.write(outputLine1+'\n')   #Game info

    for line in outputFirstAndLastLines:
        f.write(line[0]+'\n')
        f.write(line[1]+'\n')

    f.close()
    print("Created file "+f.name)


    
    


