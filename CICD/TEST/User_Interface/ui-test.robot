*** Settings ***
Library     SeleniumLibrary
#https://stackoverflow.com/questions/55104481/robot-framework-webdriverexception-message-unknown-error-chrome-failed-to-sta


*** Variables ***
${browser}              chrome
${PATH}                 /
${CHROMEDRIVER_PATH}    /usr/local/bin/chromedriver
${EXECDIR}              ${CURDIR}
${keywords_search}      เนนเสำ
${expected_result}      google
${PROTOCOL}             https
${HOSTNAME}             google.co.th


*** Test Cases ***
Search
    Open Website


*** Keywords ***
Open Website
    ${chrome_options}=    Evaluate    sys.modules['selenium.webdriver'].ChromeOptions()    sys, selenium.webdriver
    Call Method    ${chrome_options}    add_argument    --no-sandbox
    Call Method    ${chrome_options}    add_argument    --headless
    Call Method    ${chrome_options}    add_argument    --disable-dev-shm-usage
    Open Browser    ${PROTOCOL}://${HOSTNAME}${PATH}    ${browser}    options=${chrome_options}
    Page Should Contain    ""
    # Open Browser    ${PROTOCOL}://${HOSTNAME}${PATH}    ${browser}    options=${chrome_options}    executable_path=${EXECDIR}/chromedriver
    # Open Browser    ${PROTOCOL}://${HOSTNAME}${PATH}    ${browser}    options=${chrome_options}    executable_path=${CHROMEDRIVER_PATH}
    Capture Page Screenshot
