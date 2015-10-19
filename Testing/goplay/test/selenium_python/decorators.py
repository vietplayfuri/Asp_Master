from functools import wraps
import unittest
import datetime
import os

def screenshot_on_error(test):
    @wraps(test)
    def wrapper(*args, **kwargs):
        try:
            test(*args, **kwargs)
        except:
            # unittest.TestResult.addFailure(test, err)
            test_object = args[0]
            screenshot_dir = './screenshots'
            if not os.path.exists(screenshot_dir):
                os.makedirs(screenshot_dir)
            date_string = datetime.datetime.now().strftime(
                '%m%d%y-%H%M%S')
            filename = '%s/SS-%s.png' % (screenshot_dir, date_string)
            test_object.driver.get_screenshot_as_file(filename)
    return wrapper