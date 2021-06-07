HOW TO RUN TESTS USING

1. Visual Studio. FOR VISUAL REFERENCE PLEASE SEE "RunFromVisualStudio.jpg" file
----------------------------------------------------------------------------------
Steps 
- Open DeckOfCards.sln solution file. This should load test project in visual studio
- Build the solution in debug mode. Build should complete successfully.
- Select TestRun settings file from top menu - Test -> Test Settings -> Select Test Settings File. This should open file   explorer dialog box
- Navigate to project\bin\debug\RunSettings\ folder and select prd.runsettings file
- From Test Explorer window either run all tests or selected tests

2. Console. FOR MORE REFERENCE PLEASE SEE "RunFromConsole.jpg" file
-----------------------------------------------------------------------------------
Steps
- From the Solution Directory, double click on RunTest.bat file. This should automatically open command prompt and start   executing test cases
- This will also show detailed logs for each test case run and will show that NUnit test adaptor is used while running the   tests
