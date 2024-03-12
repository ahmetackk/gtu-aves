using Python.Runtime;
RunScript("connectDeneme"); // python kodunun adı dosya konumu önemli
static void RunScript(string scriptName) {
    
    Runtime.PythonDLL= @"C:\Users\Ahmet Acıkök\AppData\Local\Programs\Python\Python311\python311.dll"; //dll değiştirelecek!!
    PythonEngine.Initialize();
    using (Py.GIL())
    {
        var pythonScript = Py.Import(scriptName);
        var message = new PyInt(3);  //python fonksiyonuna gönderilecek parametre1  
        var message2 = new PyInt(4); // parametre2

        var get_mode_func = pythonScript.InvokeMethod("deneme",message,message2);  //fonksiyonun parametre ile çağrılması
       

        
        Console.WriteLine("basarılı" + get_mode_func); //python kodunun sonucunun terminale basılması
       
            }
}
