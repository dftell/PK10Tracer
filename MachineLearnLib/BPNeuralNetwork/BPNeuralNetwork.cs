using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace WolfInv.com.MachineLearnLib.BPNeuralNetwork
{
    /// <summary>
    /// BP神经网络
    /// </summary>
    public class BPNeuralNetwork : MachineLearnClass<int, int>
    {
        public override int Classify(MLInstance<int, int> instances)
        {
            throw new NotImplementedException();
        }

        public override void FillStructBySummary(int n)
        {
            throw new NotImplementedException();
        }

        public override void InitFunctions()
        {
            throw new NotImplementedException();
        }

        public override void InitTrain()
        {
            throw new NotImplementedException();
        }

        public override void Train(int IteratCnt)
        {
            throw new NotImplementedException();
        }
    }


    /**
     * BPNN.
     * 
     * @author RenaQiu
     * 
     */
    public class BP
    {
        /**
         * 
         */
        private static long serialVersionUID = 1L;
        /**
         * input vector.
         */
        private double[] input;
        /**
         * hidden layer.
         */
        private double[] hidden;
        /**
         * output layer.
         */
        private double[] output;
        /**
         * target.
         */
        private double[] target;

        /**
         * delta vector of the hidden layer .
         */
        private double[] hidDelta;
        /**
         * output layer of the output layer.
         */
        private double[] optDelta;

        /**
         * learning rate.
         */
        private double eta;
        /**
         * momentum.
         */
        private double momentum;

        /**
         * weight matrix from input layer to hidden layer.
         */
        private double[,] iptHidWeights;
        /**
         * weight matrix from hidden layer to output layer.
         */
        private double[,] hidOptWeights;

        /**
         * previous weight update.
         */
        private double[,] iptHidPrevUptWeights;
        /**
         * previous weight update.
         */
        private double[,] hidOptPrevUptWeights;

        public double optErrSum = 0d;

        public double hidErrSum = 0d;

        private Random random;

        /**
         * Constructor.
         * <p>
         * <strong>Note:</strong> The capacity of each layer will be the parameter
         * plus 1. The additional unit is used for smoothness.
         * </p>
         * 
         * @param inputSize
         * @param hiddenSize
         * @param outputSize
         * @param eta
         * @param momentum
         * @param epoch
         */
        public BP(int inputSize, int hiddenSize, int outputSize, double eta,
                double momentum)
        {
            Init(inputSize, hiddenSize, outputSize, eta, momentum);
        }

        public void Init(int inputSize, int hiddenSize, int outputSize, double eta,
                double momentum)
        {

            input = new double[inputSize + 1];
            hidden = new double[hiddenSize + 1];
            output = new double[outputSize + 1];
            target = new double[outputSize + 1];

            hidDelta = new double[hiddenSize + 1];
            optDelta = new double[outputSize + 1];

            iptHidWeights = new double[inputSize + 1, hiddenSize + 1];
            hidOptWeights = new double[hiddenSize + 1, outputSize + 1];

            random = new Random(20140106);
            randomizeWeights(iptHidWeights);
            randomizeWeights(hidOptWeights);

            iptHidPrevUptWeights = new double[inputSize + 1, hiddenSize + 1];
            hidOptPrevUptWeights = new double[hiddenSize + 1, outputSize + 1];

            this.eta = eta;
            this.momentum = momentum;
        }

        public BP(int inputSize, int hiddenSize, int outputSize)
        {
            Init(inputSize, hiddenSize, outputSize, 0.998, 0.001);
        }
        private void randomizeWeights(double[,] matrix)
        {
            for (int i = 0, len = matrix.GetLength(0); i < len; i++)
                for (int j = 0, len2 = matrix.GetLength(1); j < len2; j++)
                {
                    double real = random.NextDouble();
                    matrix[i, j] = random.NextDouble() > 0.5 ? real : -real;
                }
        }

        /**
         * Constructor with default eta = 0.25 and momentum = 0.3.
         * 
         * @param inputSize
         * @param hiddenSize
         * @param outputSize
         * @param epoch
         */


        /**
         * Entry method. The train data should be a one-dim vector.
         * 
         * @param trainData
         * @param target
         */
        public void train(double[] trainData, double[] target)
        {
            loadInput(trainData);
            loadTarget(target);
            forward();
            calculateDelta();
            adjustWeight();
        }

        /**
         * Test the BPNN.
         * 
         * @param inData
         * @return
         */
        public double[] test(double[] inData)
        {
            if (inData.Length != input.Length - 1)
            {
                throw new Exception("Size Do Not Match.");
            }

            //System.arraycopy(inData, 0, input, 1, inData.length);
            //forward();
            Array.Copy(inData, input, inData.Length);
            forward();
            return getNetworkOutput();
        }

        /**
         * Return the output layer.
         * 
         * @return
         */
        private double[] getNetworkOutput()
        {
            int len = output.Length;
            double[] temp = new double[len - 1];
            for (int i = 1; i < len; i++)
                temp[i - 1] = output[i];
            return temp;
        }

        /**
         * Load the target data.
         * 
         * @param arg
         */
        private void loadTarget(double[] arg)
        {
            if (arg.Length != target.Length - 1)
            {
                throw new Exception("Size Do Not Match.");
            }
            //System.arraycopy(arg, 0, target, 1, arg.length);
            Array.Copy(arg, target, arg.Length);
        }

        /**
         * Load the training data.
         * 
         * @param inData
         */
        private void loadInput(double[] inData)
        {
            if (inData.Length != input.Length - 1)
            {
                throw new Exception("Size Do Not Match.");
            }
            //System.arraycopy(inData, 0, input, 1, inData.length);
            Array.Copy(inData, 0, input, 1, inData.Length);
        }

        /**
         * Forward.
         * 
         * @param layer0
         * @param layer1
         * @param weight
         */
        private void forward(double[] layer0, double[] layer1, double[,] weight)
        {
            // threshold unit.
            layer0[0] = 1.0;
            for (int j = 1, len = layer1.Length; j != len; ++j)
            {
                double sum = 0;
                for (int i = 0, len2 = layer0.Length; i != len2; ++i)
                    sum += weight[i, j] * layer0[i];
                layer1[j] = sigmoid(sum);
                // layer1[j] = tansig(sum);
            }
        }

        /**
         * Forward.
         */
        private void forward()
        {
            forward(input, hidden, iptHidWeights);
            forward(hidden, output, hidOptWeights);
        }

        /**
         * Calculate output error.
         */
        private void outputErr()
        {
            double errSum = 0;
            for (int idx = 1, len = optDelta.Length; idx != len; ++idx)
            {
                double o = output[idx];
                optDelta[idx] = o * (1d - o) * (target[idx] - o);
                errSum += Math.Abs(optDelta[idx]);
            }
            optErrSum = errSum;
        }

        /**
         * Calculate hidden errors.
         */
        private void hiddenErr()
        {
            double errSum = 0;
            for (int j = 1, len = hidDelta.Length; j != len; ++j)
            {
                double o = hidden[j];
                double sum = 0;
                for (int k = 1, len2 = optDelta.Length; k != len2; ++k)
                    sum += hidOptWeights[j, k] * optDelta[k];
                hidDelta[j] = o * (1d - o) * sum;
                errSum += Math.Abs(hidDelta[j]);
            }
            hidErrSum = errSum;
        }

        /**
         * Calculate errors of all layers.
         */
        private void calculateDelta()
        {
            outputErr();
            hiddenErr();
        }

        /**
         * Adjust the weight matrix.
         * 
         * @param delta
         * @param layer
         * @param weight
         * @param prevWeight
         */
        private void adjustWeight(double[] delta, double[] layer,
                double[,] weight, double[,] prevWeight)
        {

            layer[0] = 1;
            for (int i = 1, len = delta.Length; i != len; ++i)
            {
                for (int j = 0, len2 = layer.Length; j != len2; ++j)
                {
                    double newVal = momentum * prevWeight[j, i] + eta * delta[i]
                            * layer[j];
                    weight[j, i] += newVal;
                    prevWeight[j, i] = newVal;
                }
            }
        }

        /**
         * Adjust all weight matrices.
         */
        private void adjustWeight()
        {
            adjustWeight(optDelta, hidden, hidOptWeights, hidOptPrevUptWeights);
            adjustWeight(hidDelta, input, iptHidWeights, iptHidPrevUptWeights);
        }

        /**
         * Sigmoid.
         * 
         * @param val
         * @return
         */
        private double sigmoid(double val)
        {
            return 1d / (1d + Math.Exp(-val));
        }

        private double tansig(double val)
        {
            return 2d / (1d + Math.Exp(-2 * val)) - 1;
        }
    }


    public class BPFactory
    {
        /**
         * BP神经网络元
         */
        private static BP bp;

        /**
         * 初始化一个全新的bp神经网络
         * @param inputSize
         * @param hiddenSize 
         * @param outputSize
         */
        public static void initialization(int inputSize, int hiddenSize, int outputSize)
        {
            bp = new BP(inputSize, hiddenSize, outputSize);
        }

        ///////**
        ////// * 从文件数据中读取bp神经网络
        ////// * @param file
        ////// * @throws IOException
        ////// * @throws ClassNotFoundException
        ////// */
        //////public static void initialization(File file)
        //////{

        //////    FileInputStream fi = new FileInputStream(file);
        //////    ObjectInputStream si = new ObjectInputStream(fi);
        //////    bp = (BP)si.readObject();
        //////    si.close();
        //////}

        ///////**
        ////// * 将目前的神经网络储存在指定文件
        ////// * @param file
        ////// * @throws IOException
        ////// */
        //////public static void save(File file)
        //////{
        //////    FileOutputStream fo = new FileOutputStream(file);
        //////    ObjectOutputStream so = new ObjectOutputStream(fo);
        //////    so.writeObject(bp);
        //////    so.close();
        //////}

        /**
         * 训练BP神经网络
         * @param trainData
         * @param target
         */
        public static void train(double[] trainData, double[] target)
        {
            bp.train(trainData, target);
        }

        /**
         * 要求bp神经网络返回预测值
         * @param inData
         * @return
         */
        public static double[] test(double[] inData)
        {
            return bp.test(inData);
        }
    }

}
