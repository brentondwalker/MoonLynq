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

        if (SINR_15_CQI_TU == null) return 0;

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



            if (false)
            {
                double t = (SINR - SINR_15_CQI_TU[CQI - 1, index]) /
                           (SINR_15_CQI_TU[CQI - 1, index + 1] - SINR_15_CQI_TU[CQI - 1, index]);

                int row = CQI - 1;
                int i0 = Mathf.Clamp(index - 1, 0, 14);
                int i1 = index;
                int i2 = index + 1;
                int i3 = Mathf.Clamp(index + 2, 0, 15);

                double blerInterp = CatmullRom(
                    BLER_15_CQI_TU[row, i0],
                    BLER_15_CQI_TU[row, i1],
                    BLER_15_CQI_TU[row, i2],
                    BLER_15_CQI_TU[row, i3],
                    t);

                BLER = Math.Min(Math.Max(blerInterp, 0.0), 1.0);
            }

        }

        if (BLER >= 0.1)
        {
            //Console.WriteLine($"SINR: {SINR} CQI: {CQI} SINRprec: {SINR_15_CQI_TU[CQI - 1, index]} " +
            //                  $"SINRsucc: {SINR_15_CQI_TU[CQI - 1, index + 1]} BLERprec: {BLER_15_CQI_TU[CQI - 1, index]} " +
            //                  $"BLERsucc: {BLER_15_CQI_TU[CQI - 1, index + 1]} R: {R} BLER: {BLER}");
        }

        return BLER;
    }



    double CatmullRom(double p0, double p1, double p2, double p3, double t)
    {
        double t2 = t * t;
        double t3 = t2 * t;
        return 0.5 * (
            (2.0 * p1) +
            (-p0 + p2) * t +
            (2.0 * p0 - 5.0 * p1 + 4.0 * p2 - p3) * t2 +
            (-p0 + 3.0 * p1 - 3.0 * p2 + p3) * t3
        );
    }

}
