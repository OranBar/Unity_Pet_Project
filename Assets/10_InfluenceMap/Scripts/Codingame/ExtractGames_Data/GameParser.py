import glob
from nt import chdir
import re
import pandas as pd
import os


pathname = r"E:\Unity Projects\InfluenceMap_Code_Royale_Codingame\Assets\10_InfluenceMap\Scripts\Codingame\ExtractGames_Data"
chdir(pathname)

outputLines=[]
for file in glob.glob("*.html"):
    file_content = open(file, "r").read()
    print (file)
    outputLine0 = re.findall('trustHtml"><div class="outputLine">\s?(.+?)\s?\</div', file_content)
    
    outputLine1 = re.findall('\|</div><div class="outputLine">\s?(.+?)\s?\</div', file_content)
    
 
    # print (outputLine0[0])  #game info
    # print (outputLine1[0])
    # print ( outputLine0)

    outputLines.append (outputLine0[0])  #game info
    outputLines.append( outputLine0) #all game states and pulzella

    file_name_noextension = os.path.splitext(file)[0]
    f = open(file_name_noextension+".csv","w+")
    for line in outputLine0:
        f.write(line+'\n')
    
    f.close()

    
    


