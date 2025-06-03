using UnityEngine;
using System;
using System.Collections.Generic;

public class NRMCS : MonoBehaviour
{
    public enum Modulation
    {
        QPSK,
        QAM16,
        QAM64,
        QAM256
    }

    public struct CQIelem
    {
        public Modulation ModulationType;
        public double CodeRate;

        public CQIelem(Modulation modulation, double codeRate)
        {
            ModulationType = modulation;
            CodeRate = codeRate;
        }
    }



    public struct NRMCSelem
    {
        public Modulation ModulationType;
        public double CodeRate;

        public NRMCSelem(Modulation modulation, double codeRate)
        {
            ModulationType = modulation;
            CodeRate = codeRate;
        }
    }

    public bool extended = true;

    public List<CQIelem> CQITable = new List<CQIelem>();
    public List<NRMCSelem> MCSTable = new List<NRMCSelem>();

    void Awake()
    {
        if (!extended)
        {
            CQITable.AddRange(new[]
            {
                new CQIelem(Modulation.QPSK, 0.0),
                new CQIelem(Modulation.QPSK, 78.0),
                new CQIelem(Modulation.QPSK, 120.0),
                new CQIelem(Modulation.QPSK, 193.0),
                new CQIelem(Modulation.QPSK, 308.0),
                new CQIelem(Modulation.QPSK, 449.0),
                new CQIelem(Modulation.QPSK, 602.0),
                new CQIelem(Modulation.QAM16, 378.0),
                new CQIelem(Modulation.QAM16, 490.0),
                new CQIelem(Modulation.QAM16, 616.0),
                new CQIelem(Modulation.QAM64, 466.0),
                new CQIelem(Modulation.QAM64, 567.0),
                new CQIelem(Modulation.QAM64, 666.0),
                new CQIelem(Modulation.QAM64, 772.0),
                new CQIelem(Modulation.QAM64, 873.0),
                new CQIelem(Modulation.QAM64, 948.0)
            });

            MCSTable.AddRange(new[]
            {
                new NRMCSelem(Modulation.QPSK, 120.0),
                new NRMCSelem(Modulation.QPSK, 157.0),
                new NRMCSelem(Modulation.QPSK, 193.0),
                new NRMCSelem(Modulation.QPSK, 251.0),
                new NRMCSelem(Modulation.QPSK, 308.0),
                new NRMCSelem(Modulation.QPSK, 379.0),
                new NRMCSelem(Modulation.QPSK, 449.0),
                new NRMCSelem(Modulation.QPSK, 526.0),
                new NRMCSelem(Modulation.QPSK, 602.0),
                new NRMCSelem(Modulation.QPSK, 679.0),
                new NRMCSelem(Modulation.QAM16, 340.0),
                new NRMCSelem(Modulation.QAM16, 378.0),
                new NRMCSelem(Modulation.QAM16, 434.0),
                new NRMCSelem(Modulation.QAM16, 490.0),
                new NRMCSelem(Modulation.QAM16, 553.0),
                new NRMCSelem(Modulation.QAM16, 616.0),
                new NRMCSelem(Modulation.QAM16, 658.0),
                new NRMCSelem(Modulation.QAM64, 438.0),
                new NRMCSelem(Modulation.QAM64, 466.0),
                new NRMCSelem(Modulation.QAM64, 517.0),
                new NRMCSelem(Modulation.QAM64, 567.0),
                new NRMCSelem(Modulation.QAM64, 616.0),
                new NRMCSelem(Modulation.QAM64, 666.0),
                new NRMCSelem(Modulation.QAM64, 719.0),
                new NRMCSelem(Modulation.QAM64, 772.0),
                new NRMCSelem(Modulation.QAM64, 822.0),
                new NRMCSelem(Modulation.QAM64, 873.0),
                new NRMCSelem(Modulation.QAM64, 910.0),
                new NRMCSelem(Modulation.QAM64, 948.0)
            });
        }
        else
        {
            CQITable.AddRange(new[]
            {
                new CQIelem(Modulation.QPSK, 0.0),
                new CQIelem(Modulation.QPSK, 78.0),
                new CQIelem(Modulation.QPSK, 193.0),
                new CQIelem(Modulation.QPSK, 449.0),
                new CQIelem(Modulation.QAM16, 378.0),
                new CQIelem(Modulation.QAM16, 490.0),
                new CQIelem(Modulation.QAM16, 616.0),
                new CQIelem(Modulation.QAM64, 466.0),
                new CQIelem(Modulation.QAM64, 567.0),
                new CQIelem(Modulation.QAM64, 666.0),
                new CQIelem(Modulation.QAM64, 772.0),
                new CQIelem(Modulation.QAM64, 873.0),
                new CQIelem(Modulation.QAM256, 711.0),
                new CQIelem(Modulation.QAM256, 797.0),
                new CQIelem(Modulation.QAM256, 885.0),
                new CQIelem(Modulation.QAM256, 948.0)
            });

            MCSTable.AddRange(new[]
            {
                new NRMCSelem(Modulation.QPSK, 120.0),
                new NRMCSelem(Modulation.QPSK, 193.0),
                new NRMCSelem(Modulation.QPSK, 308.0),
                new NRMCSelem(Modulation.QPSK, 449.0),
                new NRMCSelem(Modulation.QPSK, 602.0),
                new NRMCSelem(Modulation.QAM16, 378.0),
                new NRMCSelem(Modulation.QAM16, 434.0),
                new NRMCSelem(Modulation.QAM16, 490.0),
                new NRMCSelem(Modulation.QAM16, 553.0),
                new NRMCSelem(Modulation.QAM16, 616.0),
                new NRMCSelem(Modulation.QAM16, 658.0),
                new NRMCSelem(Modulation.QAM64, 466.0),
                new NRMCSelem(Modulation.QAM64, 517.0),
                new NRMCSelem(Modulation.QAM64, 567.0),
                new NRMCSelem(Modulation.QAM64, 616.0),
                new NRMCSelem(Modulation.QAM64, 666.0),
                new NRMCSelem(Modulation.QAM64, 719.0),
                new NRMCSelem(Modulation.QAM64, 772.0),
                new NRMCSelem(Modulation.QAM64, 822.0),
                new NRMCSelem(Modulation.QAM64, 873.0),
                new NRMCSelem(Modulation.QAM256, 682.5),
                new NRMCSelem(Modulation.QAM256, 711.0),
                new NRMCSelem(Modulation.QAM256, 754.0),
                new NRMCSelem(Modulation.QAM256, 797.0),
                new NRMCSelem(Modulation.QAM256, 841.0),
                new NRMCSelem(Modulation.QAM256, 885.0),
                new NRMCSelem(Modulation.QAM256, 916.5),
                new NRMCSelem(Modulation.QAM256, 948.0)
            });
        }
    }

    public int GetMinIndex(Modulation mod)
    {
        for (int i = 0; i < MCSTable.Count; i++)
            if (MCSTable[i].ModulationType == mod)
                return i;

        throw new Exception($"NRMCS: Modulation {mod} not supported in current table");
    }

    public int GetMaxIndex(Modulation mod)
    {
        for (int i = MCSTable.Count - 1; i >= 0; i--)
            if (MCSTable[i].ModulationType == mod)
                return i;

        throw new Exception($"NRMCS: Modulation {mod} not supported in current table");
    }

    public CQIelem GetCqiElem(int i)
    {
        return CQITable[i];
    }

    public NRMCSelem GetMCSElem(int i)
    {
        return MCSTable[i];
    }

    public static readonly int[] nInfoToTbs = new int[]
    {
        0,24,32,40,48,56,64,72,80,88,96,104,112,120,128,136,144,152,160,168,176,184,192,208,224,240,256,272,288,304,320,
        336,352,368,384,408,432,456,480,504,528,552,576,608,640,672,704,736,768,808,848,888,928,984,1032,1064,1128,1160,1192,1224,1256,
        1288,1320,1352,1416,1480,1544,1608,1672,1736,1800,1864,1928,2024,2088,2152,2216,2280,2408,2472,2536,2600,2664,2728,2792,2856,2976,
        3104,3240,3368,3496,3624,3752,3824
    };
}