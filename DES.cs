using System.Collections;
using System.Text;

namespace DES
{
    public class DES
    {
        public void CycleShift(BitArray bits, int shift, Shift typeOfShift)
        {
            int facticalShift = shift % bits.Length;

            if (facticalShift != shift)
            {
                Console.WriteLine("Warning! too big shift");
            }

            BitArray toShift = new BitArray(facticalShift);

            switch (typeOfShift)
            {
                case Shift.Left:
                    {
                        //extract part to be shifted
                        for (int i = 0; i < facticalShift; i++)
                        {
                            toShift[i] = bits[i];
                        }

                        //shift
                        bits.LeftShift(facticalShift);

                        //fill extracted part
                        int index = 0;
                        for (int i = bits.Length - facticalShift; i < bits.Length; i++)
                        {
                            bits[i] = toShift[index];
                            index++;
                        }
                    }
                    break;
                case Shift.Right:
                    throw new NotImplementedException("Right shift is not implemented yet");
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets part of array into new array from startIndex and to bit before endIndex
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public BitArray GetRange(BitArray bits, int startIndex, int endIndex)
        {
            BitArray range = new BitArray(endIndex - startIndex);

            if (startIndex < 0 || 
                startIndex >= bits.Length || 
                endIndex < 0 || 
                endIndex >= bits.Length || 
                startIndex >= endIndex)
                throw new ArgumentOutOfRangeException("Index of range was out of range");

            for (int i = startIndex; i < endIndex; i++)
                range[i - startIndex] = bits[i];

            return range;
        }

        public BitArray ConcatBitArrays(BitArray first, BitArray second)
        {
            BitArray concated = new BitArray(first.Length + second.Length);

            for (int i = 0; i < first.Length; i++)
                concated[i] = first[i];

            for (int i = 0; i < second.Length; i++)
                concated[i + first.Length] = second[i];

            return concated;
        }

        //initial key part separation
        int[] CD = new int[] {  57, 49, 41, 33, 25, 17, 9,
                                1, 58, 50, 42, 34, 26, 18,
                                10, 2, 59, 51, 43, 35, 27,
                                19, 11, 3, 60, 52, 44, 36,
                                63, 55, 47, 39, 31, 23, 15,
                                7, 62, 54, 46, 38, 30, 22,
                                14, 6, 61, 53, 45, 37, 29,
                                21, 13, 5, 28, 20, 12, 4 };

        //shift count for each round
        int[] shift = new int[]{ 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        //reduce round key
        int[] CP = new int[]{  14, 17, 11, 24, 1, 5,
                                3, 28, 15, 6, 21, 10,
                                23, 19, 12, 4, 26, 8,
                                16, 7, 27, 20, 13, 2,
                                41, 52, 31, 37, 47, 55,
                                30, 40, 51, 45, 33, 48,
                                44, 49, 39, 56, 34, 53,
                                46, 42, 50, 36, 29, 32 };

        // intital permutation table
        int[] IP = {   58 ,50 ,42 ,34 ,26 ,18 ,10 ,2 ,  
        60 ,52 ,44 ,36 ,28 ,20 ,12 ,4 ,
        62 ,54 ,46 ,38 ,30 ,22 ,14 ,6 ,
        64 ,56 ,48 ,40 ,32 ,24 ,16 ,8 ,
        57 ,49 ,41 ,33 ,25 ,17 ,9  ,1 ,
        59 ,51 ,43 ,35 ,27 ,19 ,11 ,3 ,
        61 ,53 ,45 ,37 ,29 ,21 ,13 ,5 ,
        63 ,55 ,47 ,39 ,31 ,23 ,15 ,7 };

        //reversed to initial permutation
        int[] IP_R = {   40 ,8  ,48 ,16 ,56 ,24 ,64 ,32 ,
        39 ,7  ,47 ,15 ,55 ,23 ,63 ,31 ,
        38 ,6  ,46 ,14 ,54 ,22 ,62 ,30 ,
        37 ,5  ,45 ,13 ,53 ,21 ,61 ,29 ,
        36 ,4  ,44 ,12 ,52 ,20 ,60 ,28 ,
        35 ,3  ,43 ,11 ,51 ,19 ,59 ,27 ,
        34 ,2  ,42 ,10 ,50 ,18 ,58 ,26 ,
        33 ,1  ,41 ,9  ,49 ,17 ,57 ,25 };

        byte[][][] S_boxes = new byte[][][]{
                                new byte[][]{
                                    new byte[]{ 14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7 },
                                    new byte[]{ 0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8 },
                                    new byte[]{ 4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0 },
                                    new byte[]{ 15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13 }
                                },
                                new byte[][]{
                                    new byte[]{ 15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10 },
                                    new byte[]{ 3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5 },
                                    new byte[]{ 0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15 },
                                    new byte[]{ 13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9 }
                                },
                                new byte[][]{
                                    new byte[]{ 10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8 },
                                    new byte[]{ 13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1 },
                                    new byte[]{ 13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7 },
                                    new byte[]{ 1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12 }
                                },
                                new byte[][]{
                                    new byte[]{ 7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15 },
                                    new byte[]{ 13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9 },
                                    new byte[]{ 10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4 },
                                    new byte[]{ 3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14 }
                                },
                                new byte[][]{
                                    new byte[]{ 2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9 },
                                    new byte[]{ 14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6 },
                                    new byte[]{ 4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14 },
                                    new byte[]{ 11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3 }
                                },
                                new byte[][]{
                                    new byte[]{ 12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11 },
                                    new byte[]{ 10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8 },
                                    new byte[]{ 9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6 },
                                    new byte[]{ 4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13 }
                                },
                                new byte[][]{
                                    new byte[]{ 4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1 },
                                    new byte[]{ 13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6 },
                                    new byte[]{ 1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2 },
                                    new byte[]{ 6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12 }
                                },
                                new byte[][]{
                                    new byte[]{ 13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7 },
                                    new byte[]{ 1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2 },
                                    new byte[]{ 7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8 },
                                    new byte[]{ 2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11 }
                                }
        };

        //expanding
        int[] E = new int[]{   32 ,1  ,2  ,3  ,4  ,5  ,
        4  ,5  ,6  ,7  ,8  ,9  ,
        8  ,9  ,10 ,11 ,12 ,13 ,
        12 ,13 ,14 ,15 ,16 ,17 ,
        16 ,17 ,18 ,19 ,20 ,21 ,
        20 ,21 ,22 ,23 ,24 ,25 ,
        24 ,25 ,26 ,27 ,28 ,29 ,
        28 ,29 ,30 ,31 ,32 ,1 };

        public byte[] GetKeyFromString(string key)
        {
            //TODO get 8 bytes from 7 bytes
            return new byte[16];
        }

        //TODO: private
        /// <summary>
        /// Check size
        /// </summary>
        /// <param name="key">Byte array key without integrity bits</param>
        /// <returns>If key is valid</returns>
        public bool IsValidKey(byte[] key)
        {
            if (key.Length != Constants.ExtendedKeySizeBytes)
                return false;

            return true;
        }

        //TODO: private
        /// <summary>
        /// Generates key for each iteration
        /// 
        /// </summary>
        /// <param name="key"> With additional bits</param>
        /// <returns></returns>
        public BitArray[] KeySchedule(byte[] key)
        {
            BitArray[] roundKeys = new BitArray[Constants.Rounds];

            BitArray initialKey = new BitArray(key);

            int halfOfKey = Constants.KeySizeBits / 2;


            BitArray cPartOfKey = new BitArray(halfOfKey);
            BitArray dPartOfKey = new BitArray(halfOfKey);

            for (int i = 0; i < Constants.KeySizeBits; i++)
            {
                if (i < halfOfKey)
                    cPartOfKey[i] = initialKey[CD[i] - 1];
                else
                    dPartOfKey[i - halfOfKey] = initialKey[CD[i] - 1];
            }

            //generate key for each round
            for (int i = 0; i < Constants.Rounds; i++)
            {
                CycleShift(cPartOfKey, shift[i], Shift.Left);
                CycleShift(dPartOfKey, shift[i], Shift.Left);

                BitArray roundSourceKey = ConcatBitArrays(cPartOfKey, dPartOfKey);
                BitArray roundKey = new BitArray(Constants.RoundKeySize);

                for (int j = 0; j < Constants.RoundKeySize; j++)
                    roundKey[j] = roundSourceKey[CP[j] - 1];

                roundKeys[i] = roundKey;
            }

            return roundKeys;
        }

        public BitArray EncryptSingleBlock(BitArray[] keys, BitArray openText)
        {
            //separate on 2 parts L R
            BitArray L = new BitArray(Constants.HalfBlockSizeBits);
            BitArray R = new BitArray(Constants.HalfBlockSizeBits);

            for (int i = 0; i < Constants.BlockSizeBits; i++)
            {
                if (i < Constants.HalfBlockSizeBits)
                    L[i] = openText[i];
                else
                    R[i - Constants.HalfBlockSizeBits] = openText[i];
            }

            //rounds
            for (int i = 0; i < Constants.Rounds; i++)
            {
                RoundEncrypt(ref L, ref R, keys[i]);
            }

            byte[] cryptedText = new byte[Constants.BlockSizeBytes];
            BitArray cryptedBits = ConcatBitArrays(L, R);

            return cryptedBits;
        }


        public BitArray DecryptSingleBlock(BitArray[] keys, BitArray cryptedText)
        {
            //separate on 2 parts L R
            BitArray L = new BitArray(Constants.HalfBlockSizeBits);
            BitArray R = new BitArray(Constants.HalfBlockSizeBits);

            for (int i = 0; i < Constants.BlockSizeBits; i++)
            {
                if (i < Constants.HalfBlockSizeBits)
                    L[i] = cryptedText[i];
                else
                    R[i - Constants.HalfBlockSizeBits] = cryptedText[i];
            }

            //rounds
            for (int i = Constants.Rounds - 1; i >= 0; i--)
            {
                RoundDecrypt(ref L, ref R, keys[i]);
            }

            byte[] decryptedText = new byte[Constants.BlockSizeBytes];
            BitArray decryptedBits = ConcatBitArrays(L, R);

            return decryptedBits;
        }

        public BitArray ApplyPermutation(BitArray bits, int[] permutation)
        {
            BitArray permutatedBits = new BitArray(bits.Length);

            for (int i = 0; i < bits.Length; i++)
                permutatedBits[i] = bits[permutation[i] - 1];

            return permutatedBits;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public void RoundEncrypt(ref BitArray L, ref BitArray R, BitArray roundKey)
        {
            BitArray afterF = F(R, roundKey);
            BitArray newR = afterF.Xor(L);
            L = R;
            R = newR;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public void RoundDecrypt(ref BitArray L, ref BitArray R, BitArray roundKey)
        {
            BitArray afterF = F(L, roundKey);
            BitArray newR = afterF.Xor(R);
            R = L;
            L = newR;
        }

        public BitArray F(BitArray R, BitArray roundKey)
        {
            BitArray expandedR = new BitArray(Constants.ExpandeHalfSizeBits);

            for (int i = 0; i < Constants.ExpandeHalfSizeBits; i++)
                expandedR[i] = R[E[i] - 1];

            BitArray xored = expandedR.Xor(roundKey);

            byte[] resultBytes = new byte[8];

            BitArray result = new BitArray(0);

            for (int i = 0; i < 8; i++)
            {
                result = ConcatBitArrays(result, UseSBox(GetRange(xored, i, i + 6), S_boxes[i]));
            }

            return result;
        }

        public BitArray UseSBox(BitArray sixBits, byte[][] sbox)
        {
            if (sixBits.Count != 6)
                throw new ArgumentException("There is no 6 bits");
            byte row = Convert.ToByte(Convert.ToByte(sixBits[5]) + Convert.ToByte(sixBits[0]) * (byte)2);
            byte column = Convert.ToByte(
                Convert.ToByte(sixBits[4]) +
                Convert.ToByte(sixBits[3]) * 2 + 
                Convert.ToByte(sixBits[2]) * 4 +
                Convert.ToByte(sixBits[2]) * 8);

            BitArray bits4 = new BitArray(4);

            byte number = sbox[row][column];
            bits4[3] = Convert.ToBoolean(number % 2);
            number /= 2;
            bits4[2] = Convert.ToBoolean(number % 2);
            number /= 2;
            bits4[1] = Convert.ToBoolean(number % 2);
            number /= 2;
            bits4[0] = Convert.ToBoolean(number % 2);

            return bits4;
        }
    }
}
