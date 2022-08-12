using RawPrint;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO.Compression;


// The line below prevents it from opening the console when run
// **THIS FILE** -> properties -> Output Type (Very Top) -> Windows Application

string REGISTRY_NAME = "CE-Print-PDF-Files";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
bool containsKey = Registry.ClassesRoot.OpenSubKey("*").OpenSubKey("shell").OpenSubKey(REGISTRY_NAME.Replace("-"," ")) != null;

RegistryKey newRK;
if (!containsKey)
{
    //gets the location of the currently running program (this program)
    string exeLoc = System.Reflection.Assembly.GetExecutingAssembly().Location;     // V The below gets the admin file
    string baseFolder = exeLoc.Substring(0, exeLoc.IndexOf("CePrintZippedFile") + "CePrintZippedFile".Length);
    string exeAdminLoc = baseFolder + @"\CeChangeRegistryAdmin\CeChangeRegistryAdmin\bin\Release\net6.0\CeChangeRegistryAdmin.exe";

    try
    {
        string userNewFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CeZippedPrinter");
        System.IO.Directory.CreateDirectory(userNewFolder);

        DirectoryInfo directory = new DirectoryInfo(baseFolder);
        if (Directory.Exists(userNewFolder))
            Directory.Delete(userNewFolder);
        string destinationDir = userNewFolder;

        foreach (string dir in Directory.GetDirectories(directory.FullName, "*", SearchOption.AllDirectories))
        {
            string dirToCreate = dir.Replace(directory.FullName, destinationDir);
            Directory.CreateDirectory(dirToCreate);
        }
        foreach (string newPath in Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(directory.FullName, destinationDir), true);


        Process process = new Process();
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = exeAdminLoc;
        process.StartInfo.Verb = "runas";
        process.StartInfo.Arguments = baseFolder + " " + REGISTRY_NAME + " " + exeLoc.Replace(".dll", ".exe");
        process.Start();
    }catch(Exception ex)
    {
        Console.WriteLine(ex + "\n" + ex.Message);
    }

    Console.WriteLine("Finished");
    Console.ReadLine();

} else {
    // Gets the File you right clicked on and the %TEMP% file path
    string ZIPPED_FILE_PATH = Environment.GetCommandLineArgs()[1];
    string TEMP_FOLDER_PATH = System.IO.Path.GetTempPath();

    // if the file is not a zip
    if (!ZIPPED_FILE_PATH.ToLower().EndsWith(".zip") || TEMP_FOLDER_PATH == null)
        return;

    // creates a new folder path in %TEMP% named "CE_PrintZippedFolder"
    string NEW_TEMP_FOLDER_PATH = System.IO.Path.Combine(TEMP_FOLDER_PATH, "CE_PrintZippedFolder");

    // Creates the NEW_TEMP_FOLDER_PATH directory and creates a random file name
    System.IO.Directory.CreateDirectory(TEMP_FOLDER_PATH);
    string fileName = System.IO.Path.GetRandomFileName();

    string newFilePath = System.IO.Path.Combine(NEW_TEMP_FOLDER_PATH, fileName);

    // extracts the contents of the zip file, if it is a pdf it will send it to the printer
#pragma warning disable CA1416 // Validate platform compatibility
    if (!System.IO.File.Exists(newFilePath))
    {
        ZipFile.ExtractToDirectory(ZIPPED_FILE_PATH, newFilePath);
        PrinterSettings printerSettings = new PrinterSettings();
        string printerName = printerSettings.PrinterName;

        foreach (string pdfFilePath in Directory.GetFileSystemEntries(newFilePath))
        {
            if (pdfFilePath.ToLower().EndsWith(".pdf"))
            {
                string pdfFileName = pdfFilePath.Split(@"\")[pdfFilePath.Split(@"\").Length - 1];

                Console.WriteLine(pdfFileName);
                //IPrinter printer = new Printer();
                //printer.PrintRawFile(printerName, pdfFilePath, pdfFileName);
            }
        }
    }
    Console.ReadLine();
}