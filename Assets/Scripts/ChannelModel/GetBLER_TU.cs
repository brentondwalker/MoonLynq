using System;
using UnityEngine;

public  class GetBLER_TU : MonoBehaviour
{
    private double[,] BLER_15_CQI_TU;
    private double[,] SINR_15_CQI_TU;

    void Start()
    {
        BLER_15_CQI_TU = BLER_Data_TU.BLER_15_CQI_TU;
        SINR_15_CQI_TU = BLER_Data_TU.SINR_15_CQI_TU;
    }


    public double GetBLER(double SINR, int MCS)
    {
        int index = -1;
        double BLER = 0.0;
        int CQI = MCS;
        //CQI = Mathf.Clamp(CQI, 1, 15);
        double R = 0.0;

        if (SINR <= SINR_15_CQI_TU[CQI - 1, 0])
        {
            BLER = 1.0;
        }
        else if (SINR >= SINR_15_CQI_TU[CQI - 1, 15])
        {
            BLER = 0.0;
        }
        else
        {
            for (int i = 0; i < 15; i++)
            {
                if (SINR >= SINR_15_CQI_TU[CQI - 1, i] && SINR < SINR_15_CQI_TU[CQI - 1, i + 1])
                {
                    index = i;
                }
            }
        }

        if (index != -1)
        {
            R = (SINR - SINR_15_CQI_TU[CQI - 1, index]) /
                (SINR_15_CQI_TU[CQI - 1, index + 1] - SINR_15_CQI_TU[CQI - 1, index]);

            BLER = BLER_15_CQI_TU[CQI - 1, index] + R * (BLER_15_CQI_TU[CQI - 1, index + 1] - BLER_15_CQI_TU[CQI - 1, index]);
        }

        if (BLER >= 0.1)
        {
            //Console.WriteLine($"SINR: {SINR} CQI: {CQI} SINRprec: {SINR_15_CQI_TU[CQI - 1, index]} " +
            //                  $"SINRsucc: {SINR_15_CQI_TU[CQI - 1, index + 1]} BLERprec: {BLER_15_CQI_TU[CQI - 1, index]} " +
            //                  $"BLERsucc: {BLER_15_CQI_TU[CQI - 1, index + 1]} R: {R} BLER: {BLER}");
        }

        return BLER;
    }
}
