using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

    public static void Save(int[] achs) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/stats.scs");
        bf.Serialize(file, achs);
        file.Close();
    }

    public static int[] Load() {
        if (File.Exists(Application.persistentDataPath + "/stats.scs")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/stats.scs", FileMode.Open);
            int[] temp = (int[])bf.Deserialize(file);
            file.Close();
            return temp;
        } else {
            return new int[] { -1 };
        }
    }
}
