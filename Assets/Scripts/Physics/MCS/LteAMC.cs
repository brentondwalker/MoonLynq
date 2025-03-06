using System;
using UnityEngine;
using static LteMCS;

public class LteAMC : MonoBehaviour
{
        public LteMCS lteMcs;


        public int dlMcsScale = 153;
        public int ulMcsScale = 156;

        private CQIelem[] cqiTable;

        public int GetItbsPerCqi(int cqi, bool isUpload)
        {
            //McsTable mcsTable = null;
            //if (dir == Direction.DL)
            //    mcsTable = dlMcsTable_;
            //else if (dir == Direction.UL || dir == Direction.D2D || dir == Direction.D2D_MULTI)
            //    mcsTable = ulMcsTable_;
            //else
            //    throw new Exception("LteAmc::getItbsPerCqi(): Unrecognized direction");

            CQIelem entry = lteMcs.CQITable[cqi];
            double rate = entry.CodeRate;

            int min = 0; 
            int max = 9;
            if (entry.ModulationType == Modulation.QAM16)
            {
                min = 10;
                max = 16;
            }
            else if (entry.ModulationType == Modulation.QAM64)
            {
                min = 17;
                max = 28;
            }

            MCSelem elem = lteMcs.MCSTable[min];
            int iTbs = elem.Index;

            for (int i = min; i <= max; i++)
            {
                elem = lteMcs.MCSTable[i];
                if (elem.Threshold <= rate)
                    iTbs = elem.Index;
                else
                    break;
            }

            return iTbs;
        }


        public int ComputeBitsOnNRbs(int cqi, int layers, int blocks, bool isUpload)
        {
            if (blocks > 110) 
                throw new Exception("LteAmc::blocks2bits(): Too many blocks");

            if (blocks == 0)
                return 0;

            int iTbs = GetItbsPerCqi(cqi, isUpload);
            Modulation mod = lteMcs.CQITable[cqi].ModulationType;
            int i = (mod == Modulation.QPSK ? 0 : (mod == Modulation.QAM16 ? 9 : (mod == Modulation.QAM64 ? 15 : 0)));
            int[] tbsVect = Itbs2Tbs(mod, layers, iTbs - i);

            return tbsVect[blocks - 1];
        }


    public float ThroughputComputation(int numBands, int numLayers, int tbs)
    {
        float TTI = 0.001f;
        float throughput = numBands * numLayers * tbs/ TTI;
        return throughput;
    }
}
