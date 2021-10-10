using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAddInfo
{
    void AddInfo(string mod_directory_path);
    void AddInfo<T>(T info);
}
