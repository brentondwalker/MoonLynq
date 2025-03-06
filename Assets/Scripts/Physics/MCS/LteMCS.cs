using UnityEngine;
using System;
using System.Collections.Generic;

public class LteMCS : MonoBehaviour
{
    public enum Modulation
    {
        QPSK,
        QAM16,
        QAM64
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

    public struct MCSelem
    {
        public Modulation ModulationType;
        public int Index;
        public double Threshold;

        public MCSelem(Modulation modulation, int index, double threshold)
        {
            ModulationType = modulation;
            Index = index;
            Threshold = threshold;
        }
    }

    public List<MCSelem> MCSTable = new List<MCSelem>{
            new MCSelem(Modulation.QPSK, 0, 86.08),
            new MCSelem(Modulation.QPSK, 1, 112.80),
            new MCSelem(Modulation.QPSK, 2, 138.65),
            new MCSelem(Modulation.QPSK, 3, 179.48),
            new MCSelem(Modulation.QPSK, 4, 219.96),
            new MCSelem(Modulation.QPSK, 5, 269.86),
            new MCSelem(Modulation.QPSK, 6, 319.55),
            new MCSelem(Modulation.QPSK, 7, 374.48),
            new MCSelem(Modulation.QPSK, 8, 428.59),
            new MCSelem(Modulation.QPSK, 9, 483.37),
            new MCSelem(Modulation.QAM16, 9, 241.69),
            new MCSelem(Modulation.QAM16, 10, 268.80),
            new MCSelem(Modulation.QAM16, 11, 308.55),
            new MCSelem(Modulation.QAM16, 12, 349.13),
            new MCSelem(Modulation.QAM16, 13, 393.42),
            new MCSelem(Modulation.QAM16, 14, 437.91),
            new MCSelem(Modulation.QAM16, 15, 468.67),
            new MCSelem(Modulation.QAM64, 15, 312.45),
            new MCSelem(Modulation.QAM64, 16, 331.66),
            new MCSelem(Modulation.QAM64, 17, 367.66),
            new MCSelem(Modulation.QAM64, 18, 403.41),
            new MCSelem(Modulation.QAM64, 19, 438.55),
            new MCSelem(Modulation.QAM64, 20, 473.85),
            new MCSelem(Modulation.QAM64, 21, 511.28),
            new MCSelem(Modulation.QAM64, 22, 549.29),
            new MCSelem(Modulation.QAM64, 23, 583.51),
            new MCSelem(Modulation.QAM64, 24, 621.36),
            new MCSelem(Modulation.QAM64, 25, 646.43),
            new MCSelem(Modulation.QAM64, 26, 749.52),
        };

    public List<CQIelem> CQITable = new List<CQIelem> {
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
        new CQIelem(Modulation.QAM64, 948.0),
    };

    private const int CQI2ITBSSIZE = 29;

    public void Rescale(double scale)
    {
        if (scale <= 0)
        {
            throw new ArgumentException($"Bad Rescaling value: {scale}");
        }

        for (int i = 0; i < CQI2ITBSSIZE; i++)
        {
            MCSTable[i] = new MCSelem(MCSTable[i].ModulationType, MCSTable[i].Index, MCSTable[i].Threshold * (168.0 / scale));
        }
    }

    public static int[] Itbs2Tbs(Modulation mod, int layers, int itbs)
    {
        int[] res = null;

        //if (layers == 1 || (txMode != TxMode.OL_SPATIAL_MULTIPLEXING && txMode != TxMode.CL_SPATIAL_MULTIPLEXING))
        if (layers == 1)
        {
            if (mod == Modulation.QPSK){ res = MCS_Data.itbs2tbs_qpsk_1[itbs]; }
            if (mod == Modulation.QAM16) { res = MCS_Data.itbs2tbs_16qam_1[itbs]; }
            if (mod == Modulation.QAM64) { res = MCS_Data.itbs2tbs_64qam_1[itbs]; }
        }
        else if (layers == 2)
        {
            if (mod == Modulation.QPSK) { res = MCS_Data.itbs2tbs_qpsk_2[itbs]; }
            if (mod == Modulation.QAM16) { res = MCS_Data.itbs2tbs_16qam_2[itbs]; }
            if (mod == Modulation.QAM64) { res = MCS_Data.itbs2tbs_64qam_2[itbs]; }
        }
        else if (layers == 4)
        {
            if (mod == Modulation.QPSK) { res = MCS_Data.itbs2tbs_qpsk_4[itbs]; }
            if (mod == Modulation.QAM16) { res = MCS_Data.itbs2tbs_16qam_4[itbs]; }
            if (mod == Modulation.QAM64) { res = MCS_Data.itbs2tbs_64qam_4[itbs]; }
        }
        else
        {
            throw new Exception("Illegal number of layers in LteAmc::Itbs2Tbs()");
        }

        if (res == null)
            throw new Exception("Invalid Level value in LteAmc::Itbs2Tbs()");

        return res;
    }
}
