using Microsoft.Win32;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CA1416 // Validate platform compatibility

string BASE_FOLDER = Environment.GetCommandLineArgs()[1];
string REGISTRY_NAME = Environment.GetCommandLineArgs()[2].Replace("-", " ");
string MAIN_EXE = Environment.GetCommandLineArgs()[3];
string CURR_EXE = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");


RegistryKey newRK;

newRK = Registry.ClassesRoot.OpenSubKey("*").OpenSubKey("shell", true).CreateSubKey(REGISTRY_NAME, true);

if (newRK != null)
{
    newRK.SetValue("", REGISTRY_NAME);
    newRK.SetValue("icon", CURR_EXE);
    RegistryKey tempkey = newRK.CreateSubKey("command");
    tempkey.SetValue("", $"\"{MAIN_EXE}\"" + "\"%1\"");
}