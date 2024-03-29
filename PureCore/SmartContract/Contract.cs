﻿using Pure.Core;
using Pure.Cryptography.ECC;
using Pure.VM;
using System;
using System.Linq;

namespace Pure.SmartContract
{
    public class Contract
    {
        public byte[] Script;
        public ContractParameterType[] ParameterList;

        public virtual bool IsStandard
        {
            get
            {
                if (Script.Length != 35) return false;
                if (Script[0] != 33 || Script[34] != (byte)OpCode.CHECKSIG)
                    return false;
                return true;
            }
        }

        private UInt160 _scriptHash;
        public virtual UInt160 ScriptHash
        {
            get
            {
                if (_scriptHash == null)
                {
                    _scriptHash = Script.ToScriptHash();
                }
                return _scriptHash;
            }
        }

        public static byte[] CreateMultiSigRedeemScript(int m, params ECPoint[] publicKeys)
        {
            if (!(1 <= m && m <= publicKeys.Length && publicKeys.Length <= 1024))
                throw new ArgumentException();
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(m);
                foreach (ECPoint publicKey in publicKeys.OrderBy(p => p))
                {
                    sb.EmitPush(publicKey.EncodePoint(true));
                }
                sb.EmitPush(publicKeys.Length);
                sb.Emit(OpCode.CHECKMULTISIG);
                return sb.ToArray();
            }
        }

        public static byte[] CreateSignatureRedeemScript(ECPoint publicKey)
        {
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(publicKey.EncodePoint(true));
                sb.Emit(OpCode.CHECKSIG);
                return sb.ToArray();
            }
        }

        public static byte[] CreateRingSignatureRedeemScript(ECPoint payloadPubKey, ECPoint viewPubKey)
        {
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(payloadPubKey.EncodePoint(true));
                sb.EmitPush(viewPubKey.EncodePoint(true));
                sb.Emit(OpCode.CHECKRINGSIG);
                return sb.ToArray();
            }
        }

        public virtual bool IsMultiSigContract()
        {
            int m, n = 0;
            int i = 0;
            if (Script.Length < 37) return false;
            if (Script[i] > (byte)OpCode.PUSH16) return false;
            if (Script[i] < (byte)OpCode.PUSH1 && Script[i] != 1 && Script[i] != 2) return false;
            switch (Script[i])
            {
                case 1:
                    m = Script[++i];
                    ++i;
                    break;
                case 2:
                    m = Script.ToUInt16(++i);
                    i += 2;
                    break;
                default:
                    m = Script[i++] - 80;
                    break;
            }
            if (m < 1 || m > 1024) return false;
            while (Script[i] == 33)
            {
                i += 34;
                if (Script.Length <= i) return false;
                ++n;
            }
            if (n < m || n > 1024) return false;
            switch (Script[i])
            {
                case 1:
                    if (n != Script[++i]) return false;
                    ++i;
                    break;
                case 2:
                    if (n != Script.ToUInt16(++i)) return false;
                    i += 2;
                    break;
                default:
                    if (n != Script[i++] - 80) return false;
                    break;
            }
            if (Script[i++] != (byte)OpCode.CHECKMULTISIG) return false;
            if (Script.Length != i) return false;
            return true;
        }
    }
}
