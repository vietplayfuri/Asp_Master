from holmium.core import Element, Elements, ElementMap, Locators, Page, Sections, Section
from selenium import webdriver

from selenium_python import global_variables as glb
from selenium_python.common import common, waitModule
import time

class table(Section):
    infos = ElementMap( Locators.CSS_SELECTOR, "tr"
                            , key=lambda el: el.find_element_by_css_selector('td:nth-child(1)').text
                            , value=lambda el: el.find_element_by_css_selector('td:nth-child(2)').text
                            )
		
class page_invoice(Page):
    table_payer = table( Locators.CSS_SELECTOR, '.payer>table>tbody', timeout = glb.int_waiting_time)
    table_transaction = table( Locators.CSS_SELECTOR, '.transactions>table>tbody', timeout = glb.int_waiting_time)
