import cv2 as cv
import numpy as np
import sys

green = np.uint8([[[sys.argv[1],sys.argv[2],sys.argv[3] ]]])
hsv_green = cv.cvtColor(green,cv.COLOR_BGR2HSV)
tmp = ""

for val in hsv_green:
    for va in val:
        for v in va:
            tmp=tmp+str(v)+","

tmp = tmp[:-1]
print(tmp)