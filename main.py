import time

import cv2 as cv
import mss
import numpy

from screeninfo import get_monitors

for m in get_monitors():
    print(str(m))

with mss.mss() as sct:
    # Part of the screen to capture
    monitor = {'top': 0, 'left': 0, 'width': 1360, 'height': 768}
    #monitor = {'top': 0, 'left': 1360, 'width': 1280, 'height': 1024}

    while 'Screen capturing':
        last_time = time.time()

        # Get raw pixels from the screen, save it to a Numpy array
        img = numpy.array(sct.grab(monitor))
        
        hsv = cv.cvtColor(img, cv.COLOR_BGR2HSV)
        lower_range = numpy.array([20,64,217], dtype=numpy.uint8)
        upper_range = numpy.array([31,196,255], dtype=numpy.uint8)
        mask = cv.inRange(hsv, lower_range, upper_range)

        # Display the picture
        cv.imshow('OpenCV/Numpy normal', mask)

        # Display the picture in grayscale
        # cv2.imshow('OpenCV/Numpy grayscale',
        #            cv2.cvtColor(img, cv2.COLOR_BGRA2GRAY))

        print('fps: {0}'.format(1 / (time.time()-last_time)))

        # Press "q" to quit
        if cv.waitKey(25) & 0xFF == ord('q'):
            cv.destroyAllWindows()
            break
