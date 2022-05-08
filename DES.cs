using System.Collections;
using System.Text;
using static DES.Constants;

namespace DES
{
    public class DES
    {
        private void CycleLeftShift(BitArray bits, int shift)
        {
            int facticalShift = shift % bits.Length;

            if (facticalShift != shift)
                Console.WriteLine("Warning! too big shift");

            BitArray toShift = new BitArray(facticalShift);

            //extract part to be shifted
            for (int i = 0; i < facticalShift; i++)
                toShift[i] = bits[i];

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

        /// <summary>
        /// Gets part of array into new array from startIndex and to bit before endIndex
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private BitArray GetRange(BitArray bits, int startIndex, int endIndex)
        {
            BitArray range = new BitArray(endIndex - startIndex);

            if (startIndex < 0 ||
                startIndex >= bits.Length ||
                endIndex < 0 ||
                startIndex >= endIndex)
                throw new ArgumentOutOfRangeException("Index of range was out of range");

            if (endIndex >= bits.Length)
                endIndex = bits.Length;

            for (int i = startIndex; i < endIndex; i++)
                range[i - startIndex] = bits[i];

            return range;
        }

        private BitArray ConcatBitArrays(BitArray first, BitArray second)
        {
            BitArray concated = new BitArray(first.Length + second.Length);

            for (int i = 0; i < first.Length; i++)
                concated[i] = first[i];

            for (int i = 0; i < second.Length; i++)
                concated[i + first.Length] = second[i];

            return concated;
        }



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
        private bool IsValidKey(string key)
        {
            if (key.Length != KeyLengthString)
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
        public BitArray[] KeySchedule(BitArray initialKey)
        {
            BitArray[] roundKeys = new BitArray[Rounds];

            int halfOfKey = KeySizeBits / 2;


            BitArray cPartOfKey = new BitArray(halfOfKey);
            BitArray dPartOfKey = new BitArray(halfOfKey);

            for (int i = 0; i < KeySizeBits; i++)
            {
                if (i < halfOfKey)
                    cPartOfKey[i] = initialKey[CD[i] - 1];
                else
                    dPartOfKey[i - halfOfKey] = initialKey[CD[i] - 1];
            }

            //generate key for each round
            for (int i = 0; i < Rounds; i++)
            {
                CycleLeftShift(cPartOfKey, shift[i]);
                CycleLeftShift(dPartOfKey, shift[i]);

                BitArray roundSourceKey = ConcatBitArrays(cPartOfKey, dPartOfKey);
                BitArray roundKey = new BitArray(RoundKeySize);

                for (int j = 0; j < RoundKeySize; j++)
                    roundKey[j] = roundSourceKey[CP[j] - 1];

                roundKeys[i] = roundKey;
            }

            return roundKeys;
        }

        public BitArray EncryptSingleBlock(BitArray[] keys, BitArray openText)
        {
            ApplyPermutation(openText, IP);

            //separate on 2 parts L R
            BitArray L = new BitArray(HalfBlockSizeBits);
            BitArray R = new BitArray(HalfBlockSizeBits);

            for (int i = 0; i < BlockSizeBits; i++)
            {
                if (i < HalfBlockSizeBits)
                    L[i] = openText[i];
                else
                    R[i - HalfBlockSizeBits] = openText[i];
            }

            //rounds
            for (int i = 0; i < Rounds; i++)
                RoundEncrypt(ref L, ref R, keys[i]);

            BitArray cryptedBits = ConcatBitArrays(L, R);

            ApplyPermutation(cryptedBits, IP);

            return cryptedBits;
        }


        public BitArray DecryptSingleBlock(BitArray[] keys, BitArray cryptedText)
        {
            ApplyPermutation(cryptedText, IP);

            //separate on 2 parts L R
            BitArray L = new BitArray(HalfBlockSizeBits);
            BitArray R = new BitArray(HalfBlockSizeBits);

            for (int i = 0; i < BlockSizeBits; i++)
            {
                if (i < HalfBlockSizeBits)
                    L[i] = cryptedText[i];
                else
                    R[i - HalfBlockSizeBits] = cryptedText[i];
            }

            //rounds
            for (int i = Rounds - 1; i >= 0; i--)
                RoundDecrypt(ref L, ref R, keys[i]);

            BitArray decryptedBits = ConcatBitArrays(L, R);

            ApplyPermutation(decryptedBits, IP_R);

            return decryptedBits;
        }

        private BitArray ApplyPermutation(BitArray bits, int[] permutation)
        {
            BitArray permutatedBits = new BitArray(bits.Length);

            for (int i = 0; i < bits.Length; i++)
                permutatedBits[i] = bits[permutation[i] - 1];

            return permutatedBits;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        private void RoundEncrypt(ref BitArray L, ref BitArray R, BitArray roundKey)
        {
            BitArray afterF = F(R, roundKey);
            BitArray newR = afterF.Xor(L);
            L = R;
            R = newR;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        private void RoundDecrypt(ref BitArray L, ref BitArray R, BitArray roundKey)
        {
            BitArray afterF = F(L, roundKey);
            BitArray newR = afterF.Xor(R);
            R = L;
            L = newR;
        }

        private BitArray F(BitArray R, BitArray roundKey)
        {
            BitArray expandedR = new BitArray(ExpandeHalfSizeBits);

            for (int i = 0; i < ExpandeHalfSizeBits; i++)
                expandedR[i] = R[E[i] - 1];

            BitArray xored = expandedR.Xor(roundKey);

            BitArray result = new BitArray(0);

            for (int i = 0; i < 8; i++)
                result = ConcatBitArrays(result, UseSBox(GetRange(xored, i, i + 6), S_boxes[i]));

            return result;
        }

        private BitArray UseSBox(BitArray sixBits, byte[][] sbox)
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

        private BitArray ExtendKeyWithParityCheckBits(string key)
        {
            BitArray shortKeyBits = new BitArray(Encoding.ASCII.GetBytes(key));
            BitArray extendedKeyBits = new BitArray(ExtendedKeySizeBits);
            bool parityBit = false;
            int injCount = 0;

            for (int i = 0; i < ExtendedKeySizeBits; i++)
            {
                if ((i + 1) % 8 == 0)
                {
                    extendedKeyBits[i] = parityBit;
                    parityBit = false;
                    injCount++;
                }
                else
                {
                    parityBit ^= shortKeyBits[i - injCount];
                    extendedKeyBits[i] = shortKeyBits[i - injCount];
                }
            }

            return extendedKeyBits;
        }


        private BitArray EncryptECB(BitArray[] blocks, BitArray[] roundKeys) 
        {
            BitArray crypted = new BitArray(0);

            for (int i = 0; i < blocks.Length; i++)
                crypted = ConcatBitArrays(crypted, EncryptSingleBlock(roundKeys, blocks[i]));

            return crypted;
        }

        private BitArray DecryptECB(BitArray[] blocks, BitArray[] roundKeys)
        {
            BitArray crypted = new BitArray(0);

            for (int i = 0; i < blocks.Length; i++)
                crypted = ConcatBitArrays(crypted, DecryptSingleBlock(roundKeys, blocks[i]));

            return crypted;
        }

        private BitArray EncryptCBC(BitArray[] blocks, BitArray[] roundKeys, BitArray C0)
        {
            BitArray crypted = new BitArray(0);

            for (int i = 0; i < blocks.Length; i++)
            {
                BitArray xored = blocks[i].Xor(C0);
                BitArray C1 = EncryptSingleBlock(roundKeys, xored);
                C0 = C1;
                crypted = ConcatBitArrays(crypted, C1);
            }

            return crypted;
        }

        private BitArray DecryptCBC(BitArray[] blocks, BitArray[] roundKeys, BitArray C0)
        {
            BitArray crypted = new BitArray(0);

            for (int i = 0; i < blocks.Length; i++)
            {
                BitArray C1 = DecryptSingleBlock(roundKeys, blocks[i]);
                BitArray xored = C0.Xor(C1);
                C0 = blocks[i];
                crypted = ConcatBitArrays(crypted, xored);
            }

            return crypted;
        }

        private BitArray EncryptCFB(BitArray[] blocks, BitArray[] roundKeys, BitArray C0)
        {
            BitArray crypted = new BitArray(0);

            for (int i = 0; i < blocks.Length; i++)
            {
                BitArray toXorWith = EncryptSingleBlock(roundKeys, C0);
                BitArray xored = toXorWith.Xor(blocks[i]);
                crypted = ConcatBitArrays(crypted, xored);
                C0 = xored;
            }

            return crypted;
        }

        private BitArray DecryptCFB(BitArray[] blocks, BitArray[] roundKeys, BitArray C0)
        {
            BitArray crypted = new BitArray(0);

            for (int i = 0; i < blocks.Length; i++)
            {
                BitArray toXorWith = EncryptSingleBlock(roundKeys, C0);
                BitArray xored = toXorWith.Xor(blocks[i]);
                crypted = ConcatBitArrays(crypted, xored);
                C0 = blocks[i];
            }

            return crypted;
        }

        private BitArray EncryptOFB(BitArray[] blocks, BitArray[] roundKeys, BitArray C0)
        {
            BitArray crypted = new BitArray(0);

            for (int i = 0; i < blocks.Length; i++)
            {
                BitArray toXorWith = EncryptSingleBlock(roundKeys, C0);
                BitArray clone = (BitArray)toXorWith.Clone();
                BitArray xored = toXorWith.Xor(blocks[i]);
                crypted = ConcatBitArrays(crypted, xored);
                C0 = clone;
            }

            return crypted;
        }

        private BitArray DecryptOFB(BitArray[] blocks, BitArray[] roundKeys, BitArray C0)
        {
            BitArray crypted = new BitArray(0);

            for (int i = 0; i < blocks.Length; i++)
            {
                BitArray toXorWith = EncryptSingleBlock(roundKeys, C0);
                BitArray clone = (BitArray)toXorWith.Clone();
                BitArray xored = toXorWith.Xor(blocks[i]);
                crypted = ConcatBitArrays(crypted, xored);
                C0 = clone;
            }

            return crypted;
        }

        public byte[] Encrypt(string openText, string key, CryptoMode cryptoMode, AddMode addMode = AddMode.ANSI, byte[]? C0_bytes = null)
            => Encrypt(Encoding.UTF8.GetBytes(openText), key, cryptoMode, addMode, C0_bytes);

        public byte[] Encrypt(byte[] openText, string key, CryptoMode cryptoMode, AddMode addMode = AddMode.ANSI, byte[]? C0_bytes = null)
        {
            if (!IsValidKey(key))
                throw new ArgumentException("Invalid key!");

            BitArray extendedKey = ExtendKeyWithParityCheckBits(key);

            BitArray[] roundKeys = KeySchedule(extendedKey);

            BitArray openTextBits = new BitArray(openText);
            BitArray[] blocks = GetBlocks(openTextBits, addMode);
            BitArray crypted = new BitArray(0);

            switch (cryptoMode)
            {
                case CryptoMode.ECB:
                    {
                        crypted = EncryptECB(blocks, roundKeys);
                        break;
                    }

                case CryptoMode.CBC:
                    {
                        if (C0_bytes is null)
                            throw new ArgumentException("IV is null");
                        BitArray C0 = new BitArray(C0_bytes);

                        crypted = EncryptCBC(blocks, roundKeys, C0);
                        break;
                    }
                case CryptoMode.CFB:
                    {
                        if (C0_bytes is null)
                            throw new ArgumentException("IV is null");

                        BitArray C0 = new BitArray(C0_bytes);

                        crypted = EncryptCFB(blocks, roundKeys, C0);
                        break;
                    }
                case CryptoMode.OFB:
                    {
                        if (C0_bytes is null)
                            throw new ArgumentException("IV is null");

                        BitArray C0 = new BitArray(C0_bytes);

                        crypted = EncryptOFB(blocks, roundKeys, C0);
                        break;
                    }
                default:
                    break;
            }

            int bitsCount = crypted.Count;

            byte[] cryptedBytes = new byte[bitsCount / 8];
            crypted.CopyTo(cryptedBytes, 0);

            return cryptedBytes;
        }

        private BitArray[] GetBlocks(BitArray text, AddMode addMode = AddMode.ANSI)
        {
            int blocksNumber = text.Count / BlockSizeBits;
            int left_numbers = text.Count % BlockSizeBits;


            if (left_numbers != 0)
                blocksNumber++;

            BitArray[] blocks = new BitArray[blocksNumber];

            for (int i = 0; i < blocksNumber; i ++)
                blocks[i] = GetRange(text, i * BlockSizeBits, (i + 1) * BlockSizeBits);

            if (left_numbers == 0)
                return blocks;

            switch (addMode)
            {
                case AddMode.ANSI:
                    {
                        byte emptyBytes = Convert.ToByte((BlockSizeBits - left_numbers) / 8); 
                        byte fullBytes = (byte)(left_numbers / 8);

                        byte[] textInBytes = new byte[BlockSizeBytes];

                        blocks[blocksNumber - 1].CopyTo(textInBytes, 0);

                        for (int i = fullBytes; i < BlockSizeBytes; i++)
                        {
                            if ( i == BlockSizeBytes - 1)
                                textInBytes[i] = emptyBytes;
                            else
                                textInBytes[i] = 0;
                        }

                        blocks[blocksNumber - 1] = new BitArray(textInBytes);

                        break;
                    }

                case AddMode.ISO:
                    {
                        Random r = new Random();

                        byte emptyBytes = Convert.ToByte((BlockSizeBits - left_numbers) / 8);
                        byte fullBytes = (byte)(left_numbers / 8);

                        byte[] textInBytes = new byte[BlockSizeBytes];

                        blocks[blocksNumber - 1].CopyTo(textInBytes, 0);

                        for (int i = fullBytes; i < BlockSizeBytes; i++)
                        {
                            if (i == BlockSizeBytes - 1)
                                textInBytes[i] = emptyBytes;
                            else
                                textInBytes[i] = (byte)r.Next(0, 255);
                        }

                        blocks[blocksNumber - 1] = new BitArray(textInBytes);

                        break;
                    }
                case AddMode.PKC:
                    {
                        byte emptyBytes = Convert.ToByte((BlockSizeBits - left_numbers) / 8);
                        byte fullBytes = (byte)(left_numbers / 8);

                        byte[] textInBytes = new byte[BlockSizeBytes];

                        blocks[blocksNumber - 1].CopyTo(textInBytes, 0);

                        for (int i = fullBytes; i < BlockSizeBytes; i++)
                            textInBytes[i] = emptyBytes;

                        blocks[blocksNumber - 1] = new BitArray(textInBytes);

                        break;
                    }
                case AddMode.ISO_EIC:
                    {
                        byte emptyBytes = Convert.ToByte((BlockSizeBits - left_numbers) / 8);
                        byte fullBytes = (byte)(left_numbers / 8);

                        byte[] textInBytes = new byte[BlockSizeBytes];

                        blocks[blocksNumber - 1].CopyTo(textInBytes, 0);

                        for (int i = fullBytes; i < BlockSizeBytes; i++)
                        {
                            if (i == 0)
                                textInBytes[i] = 128;
                            else
                                textInBytes[i] = 0;
                        }

                        blocks[blocksNumber - 1] = new BitArray(textInBytes);

                        break;
                    }
                case AddMode.None:
                    return blocks;
                default:
                    break;
            }
            return blocks;
        }

        private byte[] SolvePadding(byte[] withPadding, AddMode addMode = AddMode.ANSI)
        {
            List<byte> withoutPadding = new List<byte>();
            
            switch (addMode)
            {
                case AddMode.ANSI:
                    {
                        byte lastByte = withPadding[withPadding.Length - 1];

                        //no padding
                        if (lastByte > BlockSizeBytes || lastByte > withPadding.Length || lastByte == 0)
                            return withPadding;


                        for (int i = withPadding.Length - lastByte + 1; i < withPadding.Length - 1; i++)
                            if (withPadding[i] != 0)
                                return withPadding;

                        for (int i = 0; i < withPadding.Length - lastByte; i++)
                            withoutPadding.Add(withPadding[i]);

                        return withoutPadding.ToArray();
                    }

                case AddMode.ISO:
                    {
                        byte lastByte = withPadding[withPadding.Length - 1];

                        //no padding
                        if (lastByte > BlockSizeBytes || lastByte > withPadding.Length || lastByte == 0)
                            return withPadding;

                        for (int i = 0; i < withPadding.Length - lastByte; i++)
                            withoutPadding.Add(withPadding[i]);

                        return withoutPadding.ToArray();
                    }
                case AddMode.PKC:
                    {
                        byte lastByte = withPadding[withPadding.Length - 1];

                        //no padding
                        if (lastByte > BlockSizeBytes || lastByte > withPadding.Length || lastByte == 0)
                            return withPadding;

                        for (int i = withPadding.Length - lastByte + 1; i < withPadding.Length - 1; i++)
                            if (withPadding[i] != lastByte)
                                return withPadding;

                        for (int i = 0; i < withPadding.Length - lastByte; i++)
                            withoutPadding.Add(withPadding[i]);

                        return withoutPadding.ToArray();
                    }
                case AddMode.ISO_EIC:
                    {
                        byte lastByte = withPadding[withPadding.Length - 1];

                        //no padding
                        if (lastByte != 0)
                            return withPadding;

                        int indexOf128 = 0;

                        for (int i = withPadding.Length - 1; i >= 0; i--)
                        {
                            if (withPadding[i] == 128)
                            {
                                indexOf128 = i;
                                break;
                            }
                            else if (withPadding[i] != 0)
                                return withPadding;
                        }

                        for (int i = 0; i < indexOf128; i++)
                            withoutPadding.Add(withPadding[i]);

                        return withoutPadding.ToArray();
                    }          
                case AddMode.None:
                    break;
                default:
                    break;
            }

            return withPadding;
        }

        public byte[] Decrypt(byte[] cryptedText, string key, CryptoMode cryptoMode, AddMode addMode = AddMode.ANSI, byte[]? C0_bytes = null)
        {
            if (!IsValidKey(key))
                throw new ArgumentException("Invalid key!");

            BitArray extendedKey = ExtendKeyWithParityCheckBits(key);
            BitArray[] roundKeys = KeySchedule(extendedKey);
            BitArray cryptedTextBits = new BitArray(cryptedText);
            BitArray[] blocks = GetBlocks(cryptedTextBits, AddMode.None);
            BitArray decrypted = new BitArray(0);
            switch (cryptoMode)
            {
                case CryptoMode.ECB:
                    {
                        decrypted = DecryptECB(blocks, roundKeys);
                        break;
                    }
                case CryptoMode.CBC:
                    {
                        if (C0_bytes is null)
                            throw new ArgumentException("IV is null");

                        BitArray C0 = new BitArray(C0_bytes);

                        decrypted = DecryptCBC(blocks, roundKeys, C0);
                        break;
                    }
                case CryptoMode.CFB:
                    {
                        if (C0_bytes is null)
                            throw new ArgumentException("IV is null");

                        BitArray C0 = new BitArray(C0_bytes);

                        decrypted = DecryptCFB(blocks, roundKeys, C0);
                        break;
                    }
                case CryptoMode.OFB:
                    {
                        if (C0_bytes is null)
                            throw new ArgumentException("IV is null");

                        BitArray C0 = new BitArray(C0_bytes);

                        decrypted = DecryptOFB(blocks, roundKeys, C0);
                        break;
                    }
                default:
                    break;
            }

            int bitsCount = decrypted.Count;
            byte[] cryptedBytes = new byte[bitsCount / 8];

            decrypted.CopyTo(cryptedBytes, 0);

            return SolvePadding(cryptedBytes, addMode);
        }
    }
}
