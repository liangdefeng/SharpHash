﻿using SharpHash.Base;
using SharpHash.Interfaces;
using SharpHash.Utils;
using System;

namespace SharpHash.Crypto
{
    internal static class Global
    {
        public readonly static string InvalidHashMode = "Only \"[{0}]\" HashModes are Supported";
        public readonly static string InvalidXOFSize = "XOFSize in Bits must be Multiples of 8 & be Greater than Zero Bytes";
        public readonly static string OutputLengthInvalid = "Output Length is above the Digest Length";
        public readonly static string OutputBufferTooShort = "Output Buffer Too Short";

    } // end class Global


    internal enum HashMode
    {
        Keccak = 0x1,
        SHA3 = 0x6,
        Shake = 0x1F,
        CShake = 0x04
    }; // end enum HashMode


    internal abstract class SHA3 : BlockHash, ICryptoNotBuildIn, ITransformBlock
    {
        protected UInt64[] state = null;
        protected HashMode hash_mode;

        private readonly static UInt64[] RC = new UInt64[] {
            0x0000000000000001, 0x0000000000008082, 0x800000000000808A, 0x8000000080008000,
            0x000000000000808B, 0x0000000080000001, 0x8000000080008081, 0x8000000000008009,
            0x000000000000008A, 0x0000000000000088, 0x0000000080008009, 0x000000008000000A,
            0x000000008000808B, 0x800000000000008B, 0x8000000000008089, 0x8000000000008003,
            0x8000000000008002, 0x8000000000000080, 0x000000000000800A, 0x800000008000000A,
            0x8000000080008081, 0x8000000000008080, 0x0000000080000001, 0x8000000080008008
        };

        protected SHA3(Int32 a_hash_size)
            : base(a_hash_size, 200 - (a_hash_size * 2))
        {
            state = new UInt64[25];
        } // end constructor

        override public unsafe void Initialize()
        {
            fixed (UInt64* sPtr = state)
            {
                Utils.Utils.memset((IntPtr)sPtr, 0, state.Length * sizeof(UInt64));
            }
            
            base.Initialize();
        } // end function Initialize

        override public string Name
        {
            get
            {
                switch (hash_mode)
                {
                    case HashMode.Keccak:
                        return String.Format("{0}_{1}",
                            "Keccak", hash_size * 8);
                    case HashMode.SHA3:
                        return Name;
                    case HashMode.Shake:
                    case HashMode.CShake:
                        return String.Format("{0}_{1}_{2}", Name, "XOFSizeInBytes",
                            (this as IXOF).XOFSizeInBits >> 3);
                    default:
                        throw new ArgumentInvalidHashLibException(
                            String.Format(Global.InvalidHashMode, "Keccak, SHA3, Shake, CShake"));
                }
            }
        } // end property Name

        protected void KeccakF1600_StatePermute()
        {
            UInt64 Da, De, Di, Do, Du;
            UInt64 Aba, Abe, Abi, Abo, Abu, Aga, Age, Agi, Ago, Agu, Aka, Ake, Aki, Ako, Aku, 
                Ama, Ame, Ami, Amo, Amu, Asa, Ase, Asi, Aso, Asu, BCa, BCe, BCi, BCo, BCu,
                Eba, Ebe, Ebi, Ebo, Ebu, Ega, Ege, Egi, Ego, Egu, Eka, Eke, Eki, Eko, Eku,
                Ema, Eme, Emi, Emo, Emu, Esa, Ese, Esi, Eso, Esu;
            Int32 LRound;

            Aba = state[0];
            Abe = state[1];
            Abi = state[2];
            Abo = state[3];
            Abu = state[4];
            Aga = state[5];
            Age = state[6];
            Agi = state[7];
            Ago = state[8];
            Agu = state[9];
            Aka = state[10];
            Ake = state[11];
            Aki = state[12];
            Ako = state[13];
            Aku = state[14];
            Ama = state[15];
            Ame = state[16];
            Ami = state[17];
            Amo = state[18];
            Amu = state[19];
            Asa = state[20];
            Ase = state[21];
            Asi = state[22];
            Aso = state[23];
            Asu = state[24];

            LRound = 0;
            while (LRound < 24)
            {
                // prepareTheta
                BCa = Aba ^ Aga ^ Aka ^ Ama ^ Asa;
                BCe = Abe ^ Age ^ Ake ^ Ame ^ Ase;
                BCi = Abi ^ Agi ^ Aki ^ Ami ^ Asi;
                BCo = Abo ^ Ago ^ Ako ^ Amo ^ Aso;
                BCu = Abu ^ Agu ^ Aku ^ Amu ^ Asu;

                // thetaRhoPiChiIotaPrepareTheta(LRound  , A, E)
                Da = BCu ^ Bits.RotateLeft64(BCe, 1);
                De = BCa ^ Bits.RotateLeft64(BCi, 1);
                Di = BCe ^ Bits.RotateLeft64(BCo, 1);
                Do = BCi ^ Bits.RotateLeft64(BCu, 1);
                Du = BCo ^ Bits.RotateLeft64(BCa, 1);

                Aba = Aba ^ Da;
                BCa = Aba;
                Age = Age ^ De;
                BCe = Bits.RotateLeft64(Age, 44);
                Aki = Aki ^ Di;
                BCi = Bits.RotateLeft64(Aki, 43);
                Amo = Amo ^ Do;
                BCo = Bits.RotateLeft64(Amo, 21);
                Asu = Asu ^ Du;
                BCu = Bits.RotateLeft64(Asu, 14);
                Eba = BCa ^((~ BCe) & BCi);
                Eba = Eba ^ RC[LRound];
                Ebe = BCe ^((~ BCi) & BCo);
                Ebi = BCi ^((~ BCo) & BCu);
                Ebo = BCo ^((~ BCu) & BCa);
                Ebu = BCu ^((~ BCa) & BCe);

                Abo = Abo ^ Do;
                BCa = Bits.RotateLeft64(Abo, 28);
                Agu = Agu ^ Du;
                BCe = Bits.RotateLeft64(Agu, 20);
                Aka = Aka ^ Da;
                BCi = Bits.RotateLeft64(Aka, 3);
                Ame = Ame ^ De;
                BCo = Bits.RotateLeft64(Ame, 45);
                Asi = Asi ^ Di;
                BCu = Bits.RotateLeft64(Asi, 61);
                Ega = BCa ^((~ BCe) & BCi);
                Ege = BCe ^((~ BCi) & BCo);
                Egi = BCi ^((~ BCo) & BCu);
                Ego = BCo ^((~ BCu) & BCa);
                Egu = BCu ^((~ BCa) & BCe);

                Abe = Abe ^ De;
                BCa = Bits.RotateLeft64(Abe, 1);
                Agi = Agi ^ Di;
                BCe = Bits.RotateLeft64(Agi, 6);
                Ako = Ako ^ Do;
                BCi = Bits.RotateLeft64(Ako, 25);
                Amu = Amu ^ Du;
                BCo = Bits.RotateLeft64(Amu, 8);
                Asa = Asa ^ Da;
                BCu = Bits.RotateLeft64(Asa, 18);
                Eka = BCa ^((~ BCe) & BCi);
                Eke = BCe ^((~ BCi) & BCo);
                Eki = BCi ^((~ BCo) & BCu);
                Eko = BCo ^((~ BCu) & BCa);
                Eku = BCu ^((~ BCa) & BCe);

                Abu = Abu ^ Du;
                BCa = Bits.RotateLeft64(Abu, 27);
                Aga = Aga ^ Da;
                BCe = Bits.RotateLeft64(Aga, 36);
                Ake = Ake ^ De;
                BCi = Bits.RotateLeft64(Ake, 10);
                Ami = Ami ^ Di;
                BCo = Bits.RotateLeft64(Ami, 15);
                Aso = Aso ^ Do;
                BCu = Bits.RotateLeft64(Aso, 56);
                Ema = BCa ^((~ BCe) & BCi);
                Eme = BCe ^((~ BCi) & BCo);
                Emi = BCi ^((~ BCo) & BCu);
                Emo = BCo ^((~ BCu) & BCa);
                Emu = BCu ^((~ BCa) & BCe);

                Abi = Abi ^ Di;
                BCa = Bits.RotateLeft64(Abi, 62);
                Ago = Ago ^ Do;
                BCe = Bits.RotateLeft64(Ago, 55);
                Aku = Aku ^ Du;
                BCi = Bits.RotateLeft64(Aku, 39);
                Ama = Ama ^ Da;
                BCo = Bits.RotateLeft64(Ama, 41);
                Ase = Ase ^ De;
                BCu = Bits.RotateLeft64(Ase, 2);
                Esa = BCa ^((~ BCe) & BCi);
                Ese = BCe ^((~ BCi) & BCo);
                Esi = BCi ^((~ BCo) & BCu);
                Eso = BCo ^((~ BCu) & BCa);
                Esu = BCu ^((~ BCa) & BCe);

                // prepareTheta
                BCa = Eba ^ Ega ^ Eka ^ Ema ^ Esa;
                BCe = Ebe ^ Ege ^ Eke ^ Eme ^ Ese;
                BCi = Ebi ^ Egi ^ Eki ^ Emi ^ Esi;
                BCo = Ebo ^ Ego ^ Eko ^ Emo ^ Eso;
                BCu = Ebu ^ Egu ^ Eku ^ Emu ^ Esu;

                // thetaRhoPiChiIotaPrepareTheta(LRound+1, E, A)
                Da = BCu ^ Bits.RotateLeft64(BCe, 1);
                De = BCa ^ Bits.RotateLeft64(BCi, 1);
                Di = BCe ^ Bits.RotateLeft64(BCo, 1);
                Do = BCi ^ Bits.RotateLeft64(BCu, 1);
                Du = BCo ^ Bits.RotateLeft64(BCa, 1);

                Eba = Eba ^ Da;
                BCa = Eba;
                Ege = Ege ^ De;
                BCe = Bits.RotateLeft64(Ege, 44);
                Eki = Eki ^ Di;
                BCi = Bits.RotateLeft64(Eki, 43);
                Emo = Emo ^ Do;
                BCo = Bits.RotateLeft64(Emo, 21);
                Esu = Esu ^ Du;
                BCu = Bits.RotateLeft64(Esu, 14);
                Aba = BCa ^((~ BCe) & BCi);
                Aba = Aba ^ RC[LRound + 1];
                Abe = BCe ^((~ BCi) & BCo);
                Abi = BCi ^((~ BCo) & BCu);
                Abo = BCo ^((~ BCu) & BCa);
                Abu = BCu ^((~ BCa) & BCe);

                Ebo = Ebo ^ Do;
                BCa = Bits.RotateLeft64(Ebo, 28);
                Egu = Egu ^ Du;
                BCe = Bits.RotateLeft64(Egu, 20);
                Eka = Eka ^ Da;
                BCi = Bits.RotateLeft64(Eka, 3);
                Eme = Eme ^ De;
                BCo = Bits.RotateLeft64(Eme, 45);
                Esi = Esi ^ Di;
                BCu = Bits.RotateLeft64(Esi, 61);
                Aga = BCa ^((~ BCe) & BCi);
                Age = BCe ^((~ BCi) & BCo);
                Agi = BCi ^((~ BCo) & BCu);
                Ago = BCo ^((~ BCu) & BCa);
                Agu = BCu ^((~ BCa) & BCe);

                Ebe = Ebe ^ De;
                BCa = Bits.RotateLeft64(Ebe, 1);
                Egi = Egi ^ Di;
                BCe = Bits.RotateLeft64(Egi, 6);
                Eko = Eko ^ Do;
                BCi = Bits.RotateLeft64(Eko, 25);
                Emu = Emu ^ Du;
                BCo = Bits.RotateLeft64(Emu, 8);
                Esa = Esa ^ Da;
                BCu = Bits.RotateLeft64(Esa, 18);
                Aka = BCa ^((~ BCe) & BCi);
                Ake = BCe ^((~ BCi) & BCo);
                Aki = BCi ^((~ BCo) & BCu);
                Ako = BCo ^((~ BCu) & BCa);
                Aku = BCu ^((~ BCa) & BCe);

                Ebu = Ebu ^ Du;
                BCa = Bits.RotateLeft64(Ebu, 27);
                Ega = Ega ^ Da;
                BCe = Bits.RotateLeft64(Ega, 36);
                Eke = Eke ^ De;
                BCi = Bits.RotateLeft64(Eke, 10);
                Emi = Emi ^ Di;
                BCo = Bits.RotateLeft64(Emi, 15);
                Eso = Eso ^ Do;
                BCu = Bits.RotateLeft64(Eso, 56);
                Ama = BCa ^((~ BCe) & BCi);
                Ame = BCe ^((~ BCi) & BCo);
                Ami = BCi ^((~ BCo) & BCu);
                Amo = BCo ^((~ BCu) & BCa);
                Amu = BCu ^((~ BCa) & BCe);

                Ebi = Ebi ^ Di;
                BCa = Bits.RotateLeft64(Ebi, 62);
                Ego = Ego ^ Do;
                BCe = Bits.RotateLeft64(Ego, 55);
                Eku = Eku ^ Du;
                BCi = Bits.RotateLeft64(Eku, 39);
                Ema = Ema ^ Da;
                BCo = Bits.RotateLeft64(Ema, 41);
                Ese = Ese ^ De;
                BCu = Bits.RotateLeft64(Ese, 2);
                Asa = BCa ^((~ BCe) & BCi);
                Ase = BCe ^((~ BCi) & BCo);
                Asi = BCi ^((~ BCo) & BCu);
                Aso = BCo ^((~ BCu) & BCa);
                Asu = BCu ^((~ BCa) & BCe);

                LRound += 2;
            } // end while

            // copyToState(state, A)
            state[0] = Aba;
            state[1] = Abe;
            state[2] = Abi;
            state[3] = Abo;
            state[4] = Abu;
            state[5] = Aga;
            state[6] = Age;
            state[7] = Agi;
            state[8] = Ago;
            state[9] = Agu;
            state[10] = Aka;
            state[11] = Ake;
            state[12] = Aki;
            state[13] = Ako;
            state[14] = Aku;
            state[15] = Ama;
            state[16] = Ame;
            state[17] = Ami;
            state[18] = Amo;
            state[19] = Amu;
            state[20] = Asa;
            state[21] = Ase;
            state[22] = Asi;
            state[23] = Aso;
            state[24] = Asu;

        } // end function KeccakF1600_StatePermute

        protected override unsafe void Finish()
        {
            Int32 buffer_pos = buffer.Position;

            byte[] block = buffer.GetBytesZeroPadded();

            block[buffer_pos] = (byte)hash_mode;
            block[BlockSize - 1] = (byte)(block[BlockSize - 1] ^ 0x80);

            fixed (byte* bPtr = block)
            {
                TransformBlock((IntPtr)bPtr, block.Length, 0);
            }
            
        } // end function Finish

        override protected unsafe byte[] GetResult()
        {
            byte[] result = new byte[HashSize];

            fixed (UInt64* sPtr = state)
            {
                fixed(byte* bPtr= result)
                {
                    Converters.le64_copy((IntPtr)sPtr, 0, (IntPtr)bPtr, 0, result.Length);
                }
            }
            
            return result;
        } // end function GetResult

        override protected unsafe void TransformBlock(IntPtr a_data,
                Int32 a_data_length, Int32 a_index)
        {
            UInt64[] data = new UInt64[21];
            Int32 j, blockCount;           

            fixed (UInt64* dPtr = data)
            {
                Converters.le64_copy(a_data, a_index, (IntPtr)dPtr, 0, a_data_length);
            }

            j = 0;
            blockCount = BlockSize >> 3;
            while (j < blockCount)
            {
                state[j] = state[j] ^ data[j];
                j++;
            } // end while

            KeccakF1600_StatePermute();

            fixed (UInt64* dPtr = data)
            {
                Utils.Utils.memset((IntPtr)dPtr, 0, 18 * sizeof(UInt64));
            }

        } // end function TransformBlock

    } // end class SHA3

    internal class SHA3_224 : SHA3
    {
        public SHA3_224() :
            base((Int32)HashSizeEnum.HashSize224)
        {
            hash_mode = HashMode.SHA3;
        } // end constructor

        override public IHash Clone()
        {
            SHA3_224 HashInstance = new SHA3_224();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone
        
    } // end class SHA3_224

    internal class SHA3_256 : SHA3
    {
        public SHA3_256() :
            base((Int32)HashSizeEnum.HashSize256)
        {
            hash_mode = HashMode.SHA3;
        } // end constructor

        override public IHash Clone()
        {
            SHA3_256 HashInstance = new SHA3_256();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class SHA3_256

    internal class SHA3_384 : SHA3
    {
        public SHA3_384() :
            base((Int32)HashSizeEnum.HashSize384)
        {
            hash_mode = HashMode.SHA3;
        } // end constructor

        override public IHash Clone()
        {
            SHA3_384 HashInstance = new SHA3_384();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class SHA3_384

    internal class SHA3_512 : SHA3
    {
        public SHA3_512() :
            base((Int32)HashSizeEnum.HashSize512)
        {
            hash_mode = HashMode.SHA3;
        } // end constructor

        override public IHash Clone()
        {
            SHA3_512 HashInstance = new SHA3_512();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class SHA3_512


    internal class Keccak_224 : SHA3
    {
        public Keccak_224() :
            base((Int32)HashSizeEnum.HashSize224)
        {
            hash_mode = HashMode.Keccak;
        } // end constructor

        override public IHash Clone()
        {
            Keccak_224 HashInstance = new Keccak_224();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class SHA3_224

    internal class Keccak_256 : SHA3
    {
        public Keccak_256() :
            base((Int32)HashSizeEnum.HashSize256)
        {
            hash_mode = HashMode.Keccak;
        } // end constructor

        override public IHash Clone()
        {
            Keccak_256 HashInstance = new Keccak_256();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class Keccak_256

    internal class Keccak_288 : SHA3
    {
        public Keccak_288() :
            base((Int32)HashSizeEnum.HashSize288)
        {
            hash_mode = HashMode.Keccak;
        } // end constructor

        override public IHash Clone()
        {
            Keccak_288 HashInstance = new Keccak_288();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class Keccak_288

    internal class Keccak_384 : SHA3
    {
        public Keccak_384() :
            base((Int32)HashSizeEnum.HashSize384)
        {
            hash_mode = HashMode.Keccak;
        } // end constructor

        override public IHash Clone()
        {
            Keccak_384 HashInstance = new Keccak_384();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class Keccak_384

    internal class Keccak_512 : SHA3
    {
        public Keccak_512() :
            base((Int32)HashSizeEnum.HashSize512)
        {
            hash_mode = HashMode.Keccak;
        } // end constructor

        override public IHash Clone()
        {
            Keccak_512 HashInstance = new Keccak_512();
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class Keccak_512


    internal abstract class Shake : SHA3, IXOF
    {
        private UInt64 xofSizeInBits;

        protected UInt64 BufferPosition, DigestPosition, ShakeBufferPosition;
        protected byte[] ShakeBuffer = null;
        protected bool Finalized;

        protected Shake(Int32 a_hash_size) :
            base(a_hash_size)
        {
            ShakeBuffer = new byte[8];
            hash_mode = HashMode.Shake;
            Finalized = false;
        } // end constructor

        override public void Initialize()
        {
            base.Initialize();

            BufferPosition = 0;
            DigestPosition = 0;
            ShakeBufferPosition = 8;
            Finalized = false;
            Utils.Utils.memset(ref ShakeBuffer, 0);
        } // end function 

        override public IHashResult TransformFinal()
        {         
            byte[] temp = GetResult();
            
            Initialize();

            return new HashResult(temp);
        } // end function TransformFinal

        override protected byte[] GetResult()
        {
            UInt64 XofSizeInBytes = XOFSizeInBits >> 3;

            byte[] result = new byte[XofSizeInBytes];

            DoOutput(ref result, 0, XofSizeInBytes);

            return result;
        } // end function GetResult

        private IXOF SetXOFSizeInBitsInternal(UInt64 a_XOFSizeInBits)
        {
            UInt64 LXofSizeInBytes = a_XOFSizeInBits >> 3;

            if (((a_XOFSizeInBits & 0x07) != 0) || (LXofSizeInBytes < 1))
                throw new ArgumentInvalidHashLibException(Global.InvalidXOFSize);

            xofSizeInBits = a_XOFSizeInBits;

            return this;
        } // end function SetXOFSizeInBitsInternal

        virtual public UInt64 XOFSizeInBits {
            get { return xofSizeInBits; }
            set { SetXOFSizeInBitsInternal(value); }
        }

        virtual public void DoOutput(ref byte[] a_destination, UInt64 a_destinationOffset,
            UInt64 a_outputLength)
        {
            UInt64 DestinationOffset;
            
            if (((UInt64)a_destination.Length - a_destinationOffset) < a_outputLength)
                throw new ArgumentOutOfRangeHashLibException(Global.OutputBufferTooShort);
           
            if ((DigestPosition + a_outputLength) > (XOFSizeInBits >> 3))
                throw new ArgumentOutOfRangeHashLibException(Global.OutputLengthInvalid);

            if (!Finalized)
            {
                Finish();
                Finalized = true;
            } // end if

            DestinationOffset = a_destinationOffset;

            while (a_outputLength > 0)
            {
                if (ShakeBufferPosition >= 8)
                {
                    if ((BufferPosition * 8) >= (UInt64)BlockSize)
                    {
                        KeccakF1600_StatePermute();
                        BufferPosition = 0;
                    } // end if

                    Converters.ReadUInt64AsBytesLE(state[BufferPosition], ref ShakeBuffer, 0);
                    
                    BufferPosition++;
                    ShakeBufferPosition = 0;
                } // end if

                a_destination[DestinationOffset] = ShakeBuffer[ShakeBufferPosition];

                ShakeBufferPosition++;
                a_outputLength--;
                DigestPosition++;
                DestinationOffset++;
            } // end while

        } // end function DoOutput

    } // end class Shake

    internal class Shake_128 : Shake
    {
        public Shake_128() :
            base((Int32)HashSizeEnum.HashSize128)
        { } // end constructor

        override public IHash Clone()
        {
            // Xof Cloning
            IXOF LXof = (new Shake_128() as IXOF);
            LXof.XOFSizeInBits = (this as IXOF).XOFSizeInBits;

            // Shake_128 Cloning
            Shake_128 HashInstance = (LXof as Shake_128);
            HashInstance.BufferPosition = BufferPosition;
            HashInstance.DigestPosition = DigestPosition;
            HashInstance.ShakeBufferPosition = ShakeBufferPosition;
            HashInstance.Finalized = Finalized;

            HashInstance.ShakeBuffer = new byte[ShakeBuffer.Length];
            Utils.Utils.memcopy(ref HashInstance.ShakeBuffer, ShakeBuffer, ShakeBuffer.Length);

            // Internal SHA3 Cloning
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class Shake_128

    internal class Shake_256 : Shake
    {
        public Shake_256() :
            base((Int32)HashSizeEnum.HashSize256)
        { } // end constructor

        override public IHash Clone()
        {
            // Xof Cloning
            IXOF LXof = (new Shake_256() as IXOF);
            LXof.XOFSizeInBits = (this as IXOF).XOFSizeInBits;

            // Shake_256 Cloning
            Shake_256 HashInstance = (LXof as Shake_256);
            HashInstance.BufferPosition = BufferPosition;
            HashInstance.DigestPosition = DigestPosition;
            HashInstance.ShakeBufferPosition = ShakeBufferPosition;
            HashInstance.Finalized = Finalized;

            HashInstance.ShakeBuffer = new byte[ShakeBuffer.Length];
            Utils.Utils.memcopy(ref HashInstance.ShakeBuffer, ShakeBuffer, ShakeBuffer.Length);

            // Internal SHA3 Cloning
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class Shake_256
  

    internal abstract class CShake : Shake
    {
        protected byte[] FN, FS, InitBlock;

        /// <param name="a_hash_size">
        /// the HashSize of the underlying Shake function
        /// </param>
        /// <param name="N">
        /// the function name string, note this is reserved for use by NIST.
        /// Avoid using if not required
        /// </param>
        /// <param name="S">
        /// the customization string - available for local use
        /// </param>
        protected CShake(Int32 a_hash_size, byte[] N, byte[] S)
            : base(a_hash_size)
        {
            if (!(N == null || N.Length == 0))
            {
                FN = new byte[N.Length];
                Utils.Utils.memcopy(ref FN, N, N.Length);
            } // end if

            if (!(S == null || S.Length == 0))
            {
                FS = new byte[S.Length];
                Utils.Utils.memcopy(ref FS, S, S.Length);
            } // end if

            InitBlock = null;

            if ((FN == null || FN.Length == 0) && (FS == null || FS.Length == 0))
                hash_mode = HashMode.Shake;
            else
            {
                hash_mode = HashMode.CShake;
                InitBlock = Utils.Utils.Concat(EncodeString(N), EncodeString(S));
            } // end else
        } // end constructor

        // LeftEncode returns max 9 bytes
        private static byte[] LeftEncode(UInt64 a_input)
        {
            byte LN;
            UInt64 LV;
            Int32 LIdx;

            LN = 1;
            LV = a_input;
            LV = LV >> 8;

            while (LV != 0)
            {
                LN++;
                LV = LV >> 8;
            } // end while
            
            byte[] result = new byte[LN + 1];
            result[0] = LN;

            for (LIdx = 1; LIdx <= LN; LIdx++)
                result[LIdx] = (byte)(a_input >> (8 * (LN - LIdx)));

            return result;
        } // end function LeftEncode

        override public void Initialize()
        {
            base.Initialize();

            if (InitBlock != null)
                TransformBytes(BytePad(InitBlock, BlockSize));

        } // end function Initialize

        public static byte[] RightEncode(UInt64 a_input)
        {
            Int32 LIdx;

            byte LN = 1;
            UInt64 LV = a_input;
            LV = LV >> 8;

            while (LV != 0)
            {
                LN++;
                LV = LV >> 8;
            } // end while

            byte[] result = new byte[LN + 1];
            result[LN] = LN;

            for (LIdx = 1; LIdx <= LN; LIdx++)
                result[LIdx - 1] = (byte)(a_input >> (8 * (LN - LIdx)));

            return result;
        } // end function RightEncode

        public static byte[] BytePad(byte[] a_input, Int32 AW)
        {
            byte[] buffer = Utils.Utils.Concat(LeftEncode((UInt64)AW), a_input);
            Int32 padLength = AW - (buffer.Length % AW);

            return Utils.Utils.Concat(buffer, new byte[padLength]);
        } // end function BytePad

        public static byte[] EncodeString(byte[] a_input)
        {
            if (a_input == null || a_input.Length == 0) return LeftEncode(0);

            return Utils.Utils.Concat(LeftEncode((UInt64)a_input.Length * 8), a_input);
        } // end function EncodeString

    } // end function CShake

    internal class CShake_128 : CShake
    {
        public CShake_128(byte[] N, byte[] S) :
            base((Int32)HashSizeEnum.HashSize128, N, S)
        { } // end constructor

        override public IHash Clone()
        {
            // Xof Cloning
            IXOF LXof = (new CShake_128(FN, FS) as IXOF);
            LXof.XOFSizeInBits = (this as IXOF).XOFSizeInBits;

            // CShake_128 Cloning
            CShake_128 HashInstance = (LXof as CShake_128);

            if (!(InitBlock == null || InitBlock.Length == 0))
            {
                HashInstance.InitBlock = new byte[InitBlock.Length];
                Utils.Utils.memcopy(ref HashInstance.InitBlock, InitBlock, InitBlock.Length);
            } // end if

            HashInstance.BufferPosition = BufferPosition;
            HashInstance.DigestPosition = DigestPosition;
            HashInstance.ShakeBufferPosition = ShakeBufferPosition;
            HashInstance.Finalized = Finalized;

            HashInstance.ShakeBuffer = new byte[ShakeBuffer.Length];
            Utils.Utils.memcopy(ref HashInstance.ShakeBuffer, ShakeBuffer, ShakeBuffer.Length);


            // Internal SHA3 Cloning
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class CShake_128

    internal class CShake_256 : CShake
    {
        public CShake_256(byte[] N, byte[] S) :
            base((Int32)HashSizeEnum.HashSize256, N, S)
        { } // end constructor

        override public IHash Clone()
        {
            // Xof Cloning
            IXOF LXof = (new CShake_256(FN, FS) as IXOF);
            LXof.XOFSizeInBits = (this as IXOF).XOFSizeInBits;

            // CShake_256 Cloning
            CShake_256 HashInstance = (LXof as CShake_256);

            if (InitBlock != null)
            {
                HashInstance.InitBlock = new byte[InitBlock.Length];
                Utils.Utils.memcopy(ref HashInstance.InitBlock, InitBlock, InitBlock.Length);
            } // end if

            HashInstance.BufferPosition = BufferPosition;
            HashInstance.DigestPosition = DigestPosition;
            HashInstance.ShakeBufferPosition = ShakeBufferPosition;
            HashInstance.Finalized = Finalized;

            HashInstance.ShakeBuffer = new byte[ShakeBuffer.Length];
            Utils.Utils.memcopy(ref HashInstance.ShakeBuffer, ShakeBuffer, ShakeBuffer.Length);
          
            // Internal SHA3 Cloning
            HashInstance.buffer = buffer.Clone();
            HashInstance.processed_bytes = processed_bytes;

            HashInstance.state = new UInt64[state.Length];
            for (Int32 i = 0; i < state.Length; i++)
                HashInstance.state[i] = state[i];

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

    } // end class CShake_256


    internal abstract class KMACNotBuildInAdapter : Hash, IKMAC, IKMACNotBuildIn, ICrypto, ICryptoNotBuildIn
    {
        protected IHash hash;
        protected byte[] key, Customization;

        protected KMACNotBuildInAdapter(Int32 a_hash_size) 
            : base(a_hash_size, 200 - (a_hash_size * 2))
        {} // end constructor

        ~KMACNotBuildInAdapter()
        {
            Clear();
        } // end destructor

        override public unsafe void Initialize()
        {
            hash.Initialize();
            TransformBytes(CShake.BytePad(CShake.EncodeString(Key), BlockSize));
        } // end function Initialize

        virtual protected unsafe byte[] GetResult()
        {
            UInt64 XofSizeInBytes = (hash as IXOF).XOFSizeInBits >> 3;

            byte[] result = new byte[XofSizeInBytes];

            DoOutput(ref result, 0, XofSizeInBytes);

            return result;
        } // end function GetResult

        override public IHashResult TransformFinal()
        {
            byte[] temp = GetResult();

            Initialize();

            return new HashResult(temp);
        } // end function TransformFinal

        override public void TransformBytes(byte[] a_data, Int32 a_index, Int32 a_length)
        {
            hash.TransformBytes(a_data, a_index, a_length);
        } // end function TransformBytes

        virtual public void Clear()
        {
            Utils.Utils.memset(ref key, 0);
        } // end function Clear

        virtual public byte[] Key {
            get { return key; }
            set
            {
                if (value == null || value.Length == 0)
                    key = null;
                else
                {
                    key = new byte[value.Length];
                    Utils.Utils.memcopy(ref key, value, value.Length);
                } // end else
                    
            }
        } // end property Key

        override public string Name
        {
            get
            {
                if (this is IXOF) 
                    return String.Format("{0}_{1}_{2}", Name, "XOFSizeInBytes", 
                        (hash as IXOF).XOFSizeInBits >> 3);

                return String.Format("{0}", Name);
            }
        }

        virtual public void DoOutput(ref byte[] destination, UInt64 destinationOffset, UInt64 outputLength)
        {
            if (this is IXOF)
                TransformBytes(CShake.RightEncode(0));
            else
                TransformBytes(CShake.RightEncode((hash as IXOF).XOFSizeInBits));

            (hash as IXOF).DoOutput(ref destination, destinationOffset, outputLength);
        } // end function DoOutput

    } // end class KMACNotBuildInAdapter

    internal class KMAC128 : KMACNotBuildInAdapter
    {
        private KMAC128(IHash a_hash, byte[] a_KMACKey, byte[] a_Customization,
            UInt64 a_OutputLengthInBits)
            : base((Int32)HashSizeEnum.HashSize128)
        {
            if (!(a_KMACKey == null || a_KMACKey.Length == 0))
            {
                key = new byte[a_KMACKey.Length];
                Utils.Utils.memcopy(ref key, a_KMACKey, a_KMACKey.Length);
            } // end if

            if (!(Customization == null || Customization.Length == 0))
            {
                Customization = new byte[a_Customization.Length];
                Utils.Utils.memcopy(ref Customization, a_Customization, a_Customization.Length);
            } // end if

            hash = a_hash;
            (hash as IXOF).XOFSizeInBits = a_OutputLengthInBits;
        } // end constructor

        private KMAC128(byte[] a_KMACKey, byte[] a_Customization,
            UInt64 a_OutputLengthInBits)
        : this(new CShake_128(Converters.ConvertStringToBytes("KMAC"), a_Customization) as IHash,
                a_KMACKey, a_Customization, a_OutputLengthInBits)
        { } // end constructor

        override public IHash Clone()
        {
            // KMAC128 Cloning
            KMAC128 HashInstance = new KMAC128(hash.Clone(), Key, 
                Customization, (hash as IXOF).XOFSizeInBits);

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

        public static IKMAC CreateKMAC128(byte[] a_KMACKey, byte[] a_Customization, 
            UInt64 a_OutputLengthInBits)
        {
            return new KMAC128(a_KMACKey, a_Customization, a_OutputLengthInBits) as IKMAC;
        } // end function CreateKMAC128

    } // end class KMAC128

    internal class KMAC128XOF : KMACNotBuildInAdapter, IXOF
    {
        private KMAC128XOF(byte[] a_KMACKey, byte[] a_Customization)
            : this(new CShake_128(Converters.ConvertStringToBytes("KMAC"), a_Customization) as IHash,
                  a_KMACKey, a_Customization)
        { } // end constructor

        private KMAC128XOF(IHash a_hash, byte[] a_KMACKey, byte[] a_Customization)
            : base((Int32)HashSizeEnum.HashSize128)
        {
            if (!(a_KMACKey == null || a_KMACKey.Length == 0))
            {
                key = new byte[a_KMACKey.Length];
                Utils.Utils.memcopy(ref key, a_KMACKey, a_KMACKey.Length);
            } // end if

            if (!(Customization == null || Customization.Length == 0))
            {
                Customization = new byte[a_Customization.Length];
                Utils.Utils.memcopy(ref Customization, a_Customization, a_Customization.Length);
            } // end if

            hash = a_hash;
        } // end constructor

        override public IHash Clone()
        {
            // KMAC128XOF Cloning
            KMAC128XOF HashInstance = new KMAC128XOF(hash.Clone(), Key, Customization);
            HashInstance.XOFSizeInBits = XOFSizeInBits;

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

        private IXOF SetXOFSizeInBitsInternal(UInt64 a_XofSizeInBits)
        {
            UInt64 XofSizeInBytes = a_XofSizeInBits >> 3;

            if (((XofSizeInBytes & 0x07) != 0) || (XofSizeInBytes < 1))
                throw new ArgumentInvalidHashLibException(Global.InvalidXOFSize);

            (hash as IXOF).XOFSizeInBits = a_XofSizeInBits;

            return this;
        } // end function SetXOFSizeInBitsInternal

        virtual public UInt64 XOFSizeInBits
        {
            get { return (hash as IXOF).XOFSizeInBits; }
            set { SetXOFSizeInBitsInternal(value); }
        } // end property XOFSizeInBits

        public static IKMAC CreateKMAC128XOF(byte[] a_KMACKey, byte[] a_Customization,
            UInt64 a_XofSizeInBits)
        {
            IXOF LXof = (new KMAC128XOF(a_KMACKey, a_Customization) as IKMAC) as IXOF;
            LXof.XOFSizeInBits = a_XofSizeInBits;

            return (LXof as IHash) as IKMAC;
        } // end function CreateKMAC128XOF

    } // end class KMAC128XOF

    internal class KMAC256 : KMACNotBuildInAdapter
    {
        private KMAC256(IHash a_hash, byte[] a_KMACKey, byte[] a_Customization,
            UInt64 a_OutputLengthInBits)
            : base((Int32)HashSizeEnum.HashSize256)
        {
            if (!(a_KMACKey == null || a_KMACKey.Length == 0))
            {
                key = new byte[a_KMACKey.Length];
                Utils.Utils.memcopy(ref key, a_KMACKey, a_KMACKey.Length);
            } // end if

            if (!(Customization == null || Customization.Length == 0))
            {
                Customization = new byte[a_Customization.Length];
                Utils.Utils.memcopy(ref Customization, a_Customization, a_Customization.Length);
            } // end if

            hash = a_hash;
            (hash as IXOF).XOFSizeInBits = a_OutputLengthInBits;
        } // end constructor

        private KMAC256(byte[] a_KMACKey, byte[] a_Customization,
            UInt64 a_OutputLengthInBits)
        : this(new CShake_256(Converters.ConvertStringToBytes("KMAC"), a_Customization) as IHash,
                a_KMACKey, a_Customization, a_OutputLengthInBits)
        { } // end constructor

        override public IHash Clone()
        {
            // KMAC256 Cloning
            KMAC256 HashInstance = new KMAC256(hash.Clone(), Key,
                Customization, (hash as IXOF).XOFSizeInBits);

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

        public static IKMAC CreateKMAC256(byte[] a_KMACKey, byte[] a_Customization,
            UInt64 a_OutputLengthInBits)
        {
            return new KMAC256(a_KMACKey, a_Customization, a_OutputLengthInBits) as IKMAC;
        } // end function CreateKMA256

    } // end class KMAC256

    internal class KMAC256XOF : KMACNotBuildInAdapter, IXOF
    {
        private UInt64 xofSizeInBits;

        private KMAC256XOF(byte[] a_KMACKey, byte[] a_Customization)
            : this(new CShake_256(Converters.ConvertStringToBytes("KMAC"), a_Customization) as IHash,
                  a_KMACKey, a_Customization)
        { } // end constructor

        private KMAC256XOF(IHash a_hash, byte[] a_KMACKey, byte[] a_Customization)
            : base((Int32)HashSizeEnum.HashSize256)
        {
            if (!(a_KMACKey == null || a_KMACKey.Length == 0))
            {
                key = new byte[a_KMACKey.Length];
                Utils.Utils.memcopy(ref key, a_KMACKey, a_KMACKey.Length);
            } // end if

            if (!(Customization == null || Customization.Length == 0))
            {
                Customization = new byte[a_Customization.Length];
                Utils.Utils.memcopy(ref Customization, a_Customization, a_Customization.Length);
            } // end if

            hash = a_hash;
        } // end constructor

        override public IHash Clone()
        {
            // KMAC256XOF Cloning
            KMAC256XOF HashInstance = new KMAC256XOF(hash.Clone(), Key, Customization);
            HashInstance.XOFSizeInBits = XOFSizeInBits;

            HashInstance.BufferSize = BufferSize;

            return HashInstance;
        } // end function Clone

        private IXOF SetXOFSizeInBitsInternal(UInt64 a_XofSizeInBits)
        {
            UInt64 XofSizeInBytes = a_XofSizeInBits >> 3;

            if (((XofSizeInBytes & 0x07) != 0) || (XofSizeInBytes < 1))
                throw new ArgumentInvalidHashLibException(Global.InvalidXOFSize);

            (hash as IXOF).XOFSizeInBits = a_XofSizeInBits;

            return this;
        } // end function SetXOFSizeInBitsInternal

        virtual public UInt64 XOFSizeInBits
        {
            get { return (hash as IXOF).XOFSizeInBits; }
            set { SetXOFSizeInBitsInternal(value); }
        } // end property XOFSizeInBits

        public static IKMAC CreateKMAC256XOF(byte[] a_KMACKey, byte[] a_Customization,
            UInt64 a_XofSizeInBits)
        {
            IXOF LXof = (new KMAC256XOF(a_KMACKey, a_Customization) as IKMAC) as IXOF;
            LXof.XOFSizeInBits = a_XofSizeInBits;

            return (LXof as IHash) as IKMAC;
        } // end function CreateKMAC256XOF

    } // end class KMAC256XOF

}