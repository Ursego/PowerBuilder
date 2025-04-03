// Data, contained in a DataStore, can be saved as an Excel or text file when you are debugging.
// For that, insert a new Watch with one of the following expressions (change ids_XXX to your real DS):

ids_XXX.SaveAs("C:\aaa_" + String(Today(), "yyyymmdd") + "_" + String(Now(), "hhmmss") + "_" + ids_XXX.DataObject + ".xls", Excel!, true)
ids_XXX.SaveAs("C:\aaa_" + String(Today(), "yyyymmdd") + "_" + String(Now(), "hhmmss") + "_" + ids_XXX.DataObject + ".txt", Text!, true)

// Data will be saved:
// 1. Immediately after you have created the Watch.
// 2. Each time the script is executed while the application is in the debug mode.

// As you see, the name of the created file contains the current date and time, so you don't have to delete existing files to prevent failure when the DS is saved again.

// Th aaa_ prefix groups the created files on your C drive when sorted by name, so it's easy to delete them.
// However, saving on your C drive is a bad idea. It's better to create a special folder, and pass it instead of "C:\aaa_" (for example, "C:\dev\misc\ds_debug\").

// The last argument, passed to SaveAs(), manages displaying of columns' headers in the first line of the created file.
// Pass true ("display headers") if you want to open the file and have a look on the data.
// Pass false ("hide headers") if your intent is to import the file - to prevent an import error (the header will be interpreted as the first row of data).