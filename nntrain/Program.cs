

//var path = @"C:\All\studying\data mining\mini project\nntrain\nntrain\bin\Debug\net7.0\TrainingResults\23-12-10-12-21-18\network.nnet";

//var network = dataReader.LoadNetwork(path);

//var input = new float[17]; // TODO: normalized
//var networkPrediction = network.Forward(input);








// Number of iterations before stopping
const int epochs = 1000;

// The size of each batch of data we use
const int batchSize = 179;

// Create data reader object
var dataReader = new DataReader();

// Load the data from a file
var loadedData = dataReader.LoadData();

// Normalize the input to the neural network
loadedData.inputs = dataReader.Normalize(loadedData.inputs);

// The size of the input layer
var inputSize = 17;

// Create the neural network
INeuralNetwork network = NetworkManager.NewSequential(TensorInfo.Linear(inputSize),
    NetworkLayers.FullyConnected(inputSize, ActivationType.LeakyReLU),
    NetworkLayers.FullyConnected(8, ActivationType.LeakyReLU),
    NetworkLayers.FullyConnected(8, ActivationType.LeakyReLU),
    NetworkLayers.Softmax(3));



// Create the data that we are gonna train our model on
List<(float[] x, float[] y)> data = new();

// The number of samples we are gonna use
var count = loadedData.inputs.Length;

// Add the loaded data to the data we are gonna train our model with
for(int i = 0; i < count; i++)
    data.Add((loadedData.inputs[i], loadedData.outputs[i]));

// Get the index of the item that we are gonna use to split our data into test and training subsets
var index = Math.DivRem((int)Math.Floor(data.Count() * 0.8), batchSize, out _) * batchSize;

// Make sure that the batch size is compatible with the data size
if(!(index % batchSize == 0)) throw new Exception("data size must be a multiple of the batch size");
if(!((count - index) % batchSize == 0)) throw new Exception("data size must be a multiple of the batch size");


// Create the training set
ITrainingDataset trainingData = DatasetLoader.Training(data.Take(index), batchSize);

// Create the testing set
ITestDataset testData = DatasetLoader.Test(data.Skip(index));

// Create the cancellation token to cancel the training if we want
CancellationTokenSource cts = new CancellationTokenSource();

// Listen for cancel key press
Console.CancelKeyPress += (s, e) => cts.Cancel();

// Train the network
TrainingSessionResult result = await NetworkManager.TrainNetworkAsync(network,
    trainingData,
    TrainingAlgorithms.StochasticGradientDescent(),
    epochs,
    trainingCallback: (e) => { Console.WriteLine($"Iteration: {e.Iteration}, Accuracy: {e.Result.Accuracy}, Cost: {e.Result.Cost}"); },
    testDataset: testData, token: cts.Token);

// Save the training reports
Console.WriteLine("Saving session results...");
SaveTrainingReports(result, network);

Console.WriteLine("==========================================================================================");
Console.WriteLine("Test the neural network: ");

// Test some values from the set to see validity
var testing = data.Skip(data.Count - 10);
foreach(var test in testing)
{
    var networkPrediction = network.Forward(test.x);
    Console.WriteLine($"Expected : {dataReader.DecodeTarget(test.y)}, and got: {dataReader.DecodeTarget(networkPrediction)}");
}

Console.ReadLine();

static void SaveTrainingReports(TrainingSessionResult result, INeuralNetwork network)
{
    string timestamp = DateTime.Now.ToString("yy-MM-dd-hh-mm-ss");
    string? path = Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location));
    string dir = Path.Combine(path ?? throw new InvalidOperationException("The dll path can't be null"), "TrainingResults", timestamp);
    Directory.CreateDirectory(dir);
    File.WriteAllText(Path.Combine(dir, $"cost.py"), result.TestReports.AsPythonMatplotlibChart(TrainingReportType.Cost));
    File.WriteAllText(Path.Combine(dir, $"accuracy.py"), result.TestReports.AsPythonMatplotlibChart(TrainingReportType.Accuracy));
    network.Save(new FileInfo(Path.Combine(dir, $"network{NetworkLoader.NetworkFileExtension}")));
    File.WriteAllText(Path.Combine(dir, $"network.json"), network.SerializeMetadataAsJson());
    File.WriteAllText(Path.Combine(dir, $"report.json"), result.SerializeAsJson());
    Console.WriteLine($"Stop reason: {result.StopReason}, elapsed time: {result.TrainingTime}");
}