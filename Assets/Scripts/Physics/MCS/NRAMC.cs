using System;
using System.Collections.Generic;
using UnityEngine;
using static NRMCS;

public class NRAMC : MonoBehaviour
{
    public NRMCS NRMcs;
    public string coderateDisplay = "";
    public string modFactorDisplay = "";
    public string ninfoDisplay = "";
    public string iModDisplay = "";


    private void Start()
    {
    }


    public static uint ComputeTbsFromNinfo(double nInfo, double codeRate)
    {
        if (nInfo == 0)
            return 0;

        uint tbs = 0;
        uint _nInfo = 0;
        int n;

        if (nInfo <= 3824)
        {
            n = Math.Max(3, (int)Math.Floor(Math.Log(nInfo, 2) - 6));
            _nInfo = (uint)Math.Max(24, (1 << n) * Math.Floor(nInfo / (1 << n)));

            // get tbs from table
            int j;
            for (j = 0; j < nInfoToTbs.Length - 1; j++)
            {
                if (nInfoToTbs[j] >= _nInfo)
                    break;
            }

            tbs = (uint)nInfoToTbs[j];
        }
        else
        {
            uint C;
            n = (int)Math.Floor(Math.Log(nInfo - 24, 2) - 5);
            _nInfo = (uint)((1 << n) * Math.Round((nInfo - 24) / (1 << n)));

            if (codeRate <= 0.25)
            {
                C = (uint)Math.Ceiling((_nInfo + 24) / 3816.0);
                tbs = 8u * C * (uint)Math.Ceiling((_nInfo + 24) / (8.0 * C)) - 24;
            }
            else
            {
                if (_nInfo >= 8424)
                {
                    C = (uint)Math.Ceiling((_nInfo + 24) / 8424.0);
                    tbs = 8u * C * (uint)Math.Ceiling((_nInfo + 24) / (8.0 * C)) - 24;
                }
                else
                {
                    tbs = 8u * (uint)Math.Ceiling((_nInfo + 24) / 8.0) - 24;
                }
            }
        }

        return tbs;
    }


    public uint ComputeCodewordTbs(int layers, int cw, int cqi, bool isUpload, int symbolsPerSlot, int blocks)
    {
        NRMCSelem mcsElem = GetMcsElemPerCqi(cqi, isUpload);

        uint modFactor;
        switch (mcsElem.ModulationType)
        {
            case Modulation.QPSK: modFactor = 2; break;
            case Modulation.QAM16: modFactor = 4; break;
            case Modulation.QAM64: modFactor = 6; break;
            case Modulation.QAM256: modFactor = 8; break;
            default:
                throw new InvalidOperationException("ComputeCodewordTbs: unrecognized modulation.");
        }

        double coderate = mcsElem.CodeRate / 1024.0;
        int numRe = blocks * symbolsPerSlot * 12;
        double nInfo = numRe * coderate * modFactor * layers;
        coderateDisplay = coderate.ToString("0.000");
        modFactorDisplay = modFactor.ToString("0");
        ninfoDisplay = nInfo.ToString("0.000");



        return ComputeTbsFromNinfo(Math.Floor(nInfo), coderate);
    }

    public NRMCSelem GetMcsElemPerCqi(int cqi, bool isUpload)
    {
        NRMCS mcsTable = NRMcs;
        CQIelem entry = mcsTable.GetCqiElem(cqi);
        Modulation mod = entry.ModulationType;  
        double rate = entry.CodeRate;          
        int min = mcsTable.GetMinIndex(mod);
        int max = mcsTable.GetMaxIndex(mod);

        NRMCSelem ret = mcsTable.GetMCSElem(min);

        for (int i = min; i <= max; i++)
        {
            NRMCSelem elem = mcsTable.GetMCSElem(i);
            if (elem.CodeRate <= rate)
            { 
                ret = elem;
                iModDisplay = (i+1).ToString();

            }
            else
                break;
        }

        return ret;
    }

    public float ThroughputComputation(int numBands, int numLayers, uint tbs)
    {
        float TTI = 0.001f;
        float throughput = numBands * numLayers * tbs/ TTI;
        return throughput;
    }
}
