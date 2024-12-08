using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ProcessCommander : MonoBehaviour
{
    [Header("Test Settings")] [SerializeField]
    private string driverPath;

    [SerializeField] private string repoPath;
    [SerializeField] private string envName;
    [SerializeField] private string dataPath;
    [Range(1, 128)] [SerializeField] private int cellSize;
    [SerializeField] private string modelName;

    private Process _cmdProcess;
    private static bool _isInitialized;
    private static bool _isCommandCompleted;
    /*private Task _initTask;
    private static Task _testProTask;
    private static Task _readingTask;*/

    private readonly List<string> _initCommands = new();
    private readonly List<string> _testProCommand = new();

    private void Awake()
    {
        CloseAllCmd();
        ExecuteCmd(out _cmdProcess);
        InitCommands();
        Task.Run(() => ExecuteCmdCommand(_cmdProcess, _initCommands));
        //_initTask = new Task(() => ExecuteCmdCommand(_cmdProcess, _initCommands));
        //_initTask.Start();
        //_testProTask = new Task(() => ExecuteCmdCommand(_cmdProcess, _testProCommand));
        Task.Run(() => ReadOutput(_cmdProcess));
        //_readingTask = new Task(() => ReadOutput(_cmdProcess));
        //_readingTask.Start();
    }

    private void InitCommands()
    {
        _initCommands.Add($"{driverPath}:");
        _initCommands.Add($"cd {repoPath}");
        _initCommands.Add($"conda activate {envName}");
        _testProCommand.Add(
            $"python test_pro.py --input ./datasets/{dataPath}/Input --cell_size {cellSize} --model_name {modelName}");
    }

    private void UpdateCommands()
    {
        _initCommands.Clear();
        _testProCommand.Clear();
        InitCommands();
    }

    private void ExecuteCmd(out Process process)
    {
        _isInitialized = false;
        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe")
        {
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        process = Process.Start(processInfo);

        if (process == null)
        {
            Debug.LogError($"CMD Process could not executed");
            return;
        }

        _isInitialized = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_isInitialized && _isCommandCompleted/*&& _testProTask.Status != TaskStatus.Running*/)
            {
                _isCommandCompleted = false;
                UpdateCommands();
                Task.Run(() => ExecuteCmdCommand(_cmdProcess, _testProCommand));
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCmd(_cmdProcess);
            _cmdProcess = null;
        }
    }
    
    private static void ExecuteCmdCommand(Process process, List<string> commands)
    {
        if (process != null)
        {
            foreach (var command in commands)
            {
                print($"Run command: {command}");
                process.StandardInput.WriteLine(command);
            }
            process.StandardInput.Flush();
        }

        _isCommandCompleted = true;
    }

    private static void ReadOutput(Process process)
    {
        while (true)
        {
            string tmpOutput = process.StandardOutput.ReadLine();
            if (!string.IsNullOrEmpty(tmpOutput))
            {
                print($"output: {tmpOutput}");
            }
        }
    }

    void OnDestroy()
    {
        CloseAllCmd();
    }

    void CloseAllCmd()
    {
        foreach (var process in Process.GetProcessesByName("cmd"))
        {
            try
            {
                if (process is null) continue;

                process.Kill();
                process.WaitForExit();
                process.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error closing cmd process: {e}");
            }
        }
    }

    private void ExitCmd(Process process)
    {
        try
        {
            if (process is null) return;
            process.Kill();
            process.WaitForExit();
            process.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while exiting cmd process: {e}");
        }
    }
}