﻿///////////////////////////////////////////////////////////////////////
/// SharpHash Library
/// Copyright(c) 2019 - 2020  Mbadiwe Nnaemeka Ronald
/// Github Repository <https://github.com/ron4fun/SharpHash>
///
/// The contents of this file are subject to the
/// Mozilla Public License Version 2.0 (the "License");
/// you may not use this file except in
/// compliance with the License. You may obtain a copy of the License
/// at https://www.mozilla.org/en-US/MPL/2.0/
///
/// Software distributed under the License is distributed on an "AS IS"
/// basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
/// the License for the specific language governing rights and
/// limitations under the License.
///
/// Acknowledgements:
///
/// Thanks to Ugochukwu Mmaduekwe (https://github.com/Xor-el) for his creative
/// development of this library in Pascal/Delphi (https://github.com/Xor-el/HashLib4Pascal).
///
/// Also, I will like to thank Udezue Chukwunwike (https://github.com/IzarchTech) for
/// his contributions to the growth and development of this library.
///
////////////////////////////////////////////////////////////////////////

using System;
using SharpHash.Base;
using SharpHash.Utils;
using SharpHash.Interfaces.IBlake2BConfigurations;

namespace SharpHash.Crypto.Blake2BConfigurations
{

    public sealed class Blake2BConfig : IBlake2BConfig
    {
        private Int32 hash_size;
        public Int32 HashSize {
            get => hash_size;
            set {
                ValidateHashSize(value);
                hash_size = value;
            }
        }

        private byte[] personalisation;
        public byte[] Personalisation {
            get => personalisation;
            set {
                ValidatePersonalisationLength(value);
                personalisation = value;
            }
        }

        private byte[] salt;
        public byte[]  Salt {
            get => salt;
            set {
                ValidateSaltLength(value);
                salt = value;
            }
        }

        private byte[] key;
        public byte[] Key {
            get => key;
            set {
                ValidateKeyLength(value);
                key = value;
            }
        }


        public static readonly string InvalidHashSize = "BLAKE2B HashSize must  of the following [1 .. 64], \"{0}\"";
        public static readonly string InvalidKeyLength = "\"Key\" Length Must Not Be Greatebe restricted to oner Than 64, \"{0}\"";
        public static readonly string InvalidPersonalisationLength = "\"Personalisation\" Length Must Be Equal To 16, \"{0}\"";
        public static readonly string InvalidSaltLength = "\"Salt\" Length Must Be Equal To 16, \"{0}\"";

        public Blake2BConfig(HashSizeEnum a_hash_size = HashSizeEnum.HashSize512)
	    {
		    ValidateHashSize((Int32)a_hash_size);
            hash_size = (Int32)a_hash_size;
	    }

        public Blake2BConfig(Int32 a_hash_size)
        {
            ValidateHashSize(a_hash_size);
            hash_size = a_hash_size;
        }

        ~Blake2BConfig()
        {
            Clear();
        }

        public static Blake2BConfig DefaultConfig => new Blake2BConfig();

        public IBlake2BConfig Clone()
        {
            Blake2BConfig result = new Blake2BConfig(HashSize);

            result.Key = Key.DeepCopy();
            result.Personalisation = Personalisation.DeepCopy();
            result.Salt = Salt.DeepCopy();

            return result;
        }

	    public void Clear() 
	    {
            ArrayUtils.ZeroFill(ref key);
	    }

        private void ValidateHashSize(Int32 a_hash_size)
        {
            if (!(a_hash_size > 0 && a_hash_size <= 64) || ((a_hash_size * 8) & 7) != 0)
                throw new ArgumentHashLibException(String.Format(InvalidHashSize, a_hash_size));
        }

        private void ValidateKeyLength(byte[] a_Key)
        {
            Int32 KeyLength;

            if (!a_Key.Empty())
            {
                KeyLength = a_Key.Length;
                if (KeyLength > 64)
                    throw new ArgumentOutOfRangeHashLibException(String.Format(InvalidKeyLength, KeyLength));
            }

        }

        private void ValidatePersonalisationLength(byte[] a_Personalisation)
        {
            Int32 PersonalisationLength;

            if (!a_Personalisation.Empty())
            {
                PersonalisationLength = a_Personalisation.Length;
                if (PersonalisationLength != 16)
                    throw new ArgumentOutOfRangeHashLibException(String.Format(InvalidPersonalisationLength, PersonalisationLength));
            }
        }

        private void ValidateSaltLength(byte[] a_Salt)
        {
            Int32 SaltLength;

            if (!a_Salt.Empty())
            {
                SaltLength = a_Salt.Length;
                if (SaltLength != 16)
                    throw new ArgumentOutOfRangeHashLibException(String.Format(InvalidSaltLength, SaltLength));
            }
        }

    } // end class Blake2BConfig
}
