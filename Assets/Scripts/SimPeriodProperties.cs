using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 public class SimPeriodProperties: Object
 {
     private double mean;
     private double stddev;
     private double simperiod;
     private int[] numOfIntersections;
     private int[] numOfRoadNodes;
     private int[] numOfPointsOfInterest;

     public double Mean
     {
         get
         {
             return mean;
         }
         set
         {
             mean = value;
         }
     }

     public double Stddev
     {
         get
         {
             return stddev;
         }
         set
         {
             stddev = value;
         }
     }

     public double Simperiod
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

     public int[] NumOfIntersections
     {
         get
         {
             return numOfIntersections;
         }
         set
         {
             numOfIntersections = value;
         }
     }

     public int[] NumOfRoadNodes
     {
         get
         {
             return numOfRoadNodes;
         }
         set
         {
             numOfRoadNodes = value;
         }
     }

     public int[] NumOfPointsOfInterest
     {
         get
         {
             return numOfPointsOfInterest;
         }
         set
         {
             numOfPointsOfInterest = value;
         }
     }
 }
