using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Windows.Markup;

namespace nntrain;

public class DataReader
{


    public INeuralNetwork LoadNetwork(string filePath)
    {
        FileInfo file = new FileInfo(filePath);
        INeuralNetwork network = NetworkLoader.TryLoad(file, ExecutionModePreference.Cpu);
        return network;
    }


    public float[] EncodeTarget(string target) => target switch
    {
        "Conservative Spenders" => new float[] { 1, 0, 0 },
        "Balanced Spenders" => new float[] { 0, 1, 0 },
        "Active Spenders" => new float[] { 0, 0, 1 },
        _ => throw new Exception()
    };

    public string DecodeTarget(float[] encoded)
    {
        var item1 = encoded[0];// "Conservative Spenders"
        var item2 = encoded[1];// "Balanced Spenders"
        var item3 = encoded[2];// "Active Spenders"

        if(item1 > item2 && item1 > item3)
            return "Conservative Spenders";
        
        if(item2 > item1 && item2 > item3)
            return "Balanced Spenders";
        
        return "Active Spenders";
    }

    public (float[][] inputs, float[][] outputs) LoadData()
    {
        var dataPath = @"C:\All\studying\data mining\mini project\nntrain\nntrain\ccgeneral-Labeled.csv";
        var separator = ',';
        var columns = Test(dataPath, separator);

        var target = columns.FirstOrDefault(x => x.label == "Cluster");
        columns.Remove(target);


        var inputs = new float[columns.First().data.Count][];

        for(int i = 0; i < inputs.Length; i++)
            inputs[i] = new float[columns.Count];

        for(int i = 0; i < columns.First().data.Count; i++)
        {
            for(int j = 0; j < columns.Count; j++)
            {
                inputs[i][j] = float.Parse(columns[j].data[i]);
            }
        }

        var outputs = target.data.Select(x => EncodeTarget(x)).ToArray();

        return (inputs, outputs);

        //var toHotEncodeColumns = new List<(string label, List<string> distinctValues)>();
        //var threshold = 20;
        //foreach(var col in columns)
        //{
        //    var distinctValues = col.data.Distinct().ToList();

        //Console.Write($"The column '{col.label}' has {distinctValues.Count} distinct values");
        //if(distinctValues.Count < 70)
        //    Console.Write($" which are: {string.Join(", ", distinctValues)}");
        //Console.WriteLine();

        //    if(distinctValues.Count < threshold)
        //    {
        //        toHotEncodeColumns.Add((col.label, distinctValues));
        //    }
        //}

    }

    public float[][] Normalize(float[][] input)
    {
        float[][] output = new float[input.Length][];
        for(int i = 0; i < input.Length; i++)
            output[i] = new float[input.First().Length];

        for(var col = 0; col < input.First().Length; col++)
        {
            var max = float.MinValue;
            var min = float.MaxValue;

            for(var row = 0; row < input.Length; row++)
            {
                var current = input[row][col];
                if(current > max) max = current;
                if(current < min) min = current;
            }

            for(var row = 0; row < input.Length; row++)
                output[row][col] = (input[row][col] - min) / (max - min);
        }
        return output;
    }


    public List<(string label, List<string> data)> Test(string dataPath, char separator)
    {
        //var dataPath = @"C:\All\studying\data mining\mini project\nntrain\nntrain\ObesityDataSet.csv";
        var fileContent = File.ReadAllText(dataPath);

        var lines = fileContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var firstLine = lines.First();
        var colNames = firstLine.Split(separator);

        List<(string label, List<string> data)> columns = new();

        foreach(var col in colNames)
            columns.Add((col, new List<string>(lines.Length - 1)));

        for(var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];

            var values = line.Split(separator);

            for(var j = 0; j < values.Length; j++)
            {
                var value = values[j];
                columns[j].data.Add(value);
            }
        }

        return columns;
    }


    public void ReadData()
    {
        var dataPath = @"C:\All\studying\data mining\mini project\nntrain\nntrain\ObesityDataSet.csv";
        var uniqueValues = new Dictionary<string, HashSet<string>>();

        var fileContent = File.ReadAllText(dataPath);

        var separator = ',';
        var lines = fileContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var firstLine = lines.First();
        var cols = firstLine.Split(separator);
        foreach(var col in cols) uniqueValues.Add(col, new HashSet<string>());

        for(var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var values = line.Split(separator);

            for(var j = 0; j < values.Length; j++)
            {
                var value = values[j];
                uniqueValues[cols[j]].Add(value);
            }
        }



        //   Gender    Age    Height Weight family_history_with_overweight FAVC      FCVC  NCP   CAEC           SMOKE    CH2O  SCC      FAF   TUE   CALC           MTRANS            NObeyesdad
        // [ | 1, 1 |  | 1 |  | 1 |  | 1 |  | 1, 1 |                       | 1, 1 |  | 1 | | 1 | | 1, 1, 1, 1 | | 1, 1 | | 1 | | 1, 1 | | 1 | | 1 | | 1, 1, 1, 1 | | 1, 1, 1, 1, 1 | | 1, 1, 1, 1, 1, 1, 1 |]

        // TODO: normalize values

        // one hot encode each categorical column
    }

}
