namespace nntrain.core;

public class BL
{
    public static float[] EncodeTarget(string target) => target switch
    {
        "Conservative Spenders" => new float[] { 1, 0, 0 },
        "Balanced Spenders" => new float[] { 0, 1, 0 },
        "Active Spenders" => new float[] { 0, 0, 1 },
        _ => throw new Exception()
    };


    public static string DecodeTarget(float[] encoded)
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

}
