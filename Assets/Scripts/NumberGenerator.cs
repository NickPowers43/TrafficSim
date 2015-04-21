using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class NumberGenerator 
{
    private Random random = new Random();
    private double mean;
    private double median;
    private double stddev;
    private double simperiod;
    private double accidentprobability;

    public double SimPeriod
    {
        get
        {
            return simperiod;
        }
        set
        {
            simperiod = value;
        }
    }

    public double AccidentProbability
    {
        get 
        {
            return accidentprobability;
        }
        set 
        {
            accidentprobability = value;
        }
    }

    public double setAccidentProbability(double prob){
        if (prob <= 1.0)
        {
            prob = NormalDistribution(accidentprobability);
            return prob;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    

    public double setSimulationPeriod(double simulation){
        simulation = NormalDistribution(simperiod);
        return simulation;
    }

    public double NormalDistribution(double zscore){
        double z = (zscore * zscore) / 2;
        double normal = (1 / Math.Sqrt(2 * Math.PI)) * (Math.Exp(z));

        return normal;
    }
}
