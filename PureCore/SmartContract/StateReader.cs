﻿using Pure.Core;
using Pure.Cryptography.ECC;
using Pure.VM;
using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Pure.SmartContract
{
    public class StateReader : InteropService
    {
        public event EventHandler<NotifyEventArgs> Notify;
        public event EventHandler<LogEventArgs> Log;

        public static readonly StateReader Default = new StateReader();

        public StateReader()
        {
            Register("Quras.Runtime.GetTrigger", Runtime_GetTrigger);
            Register("Quras.Runtime.CheckWitness", Runtime_CheckWitness);
            Register("Quras.Runtime.Notify", Runtime_Notify);
            Register("Quras.Runtime.Log", Runtime_Log);
            Register("Quras.Blockchain.GetHeight", Blockchain_GetHeight);
            Register("Quras.Blockchain.GetHeader", Blockchain_GetHeader);
            Register("Quras.Blockchain.GetBlock", Blockchain_GetBlock);
            Register("Quras.Blockchain.GetTransaction", Blockchain_GetTransaction);
            Register("Quras.Blockchain.GetAccount", Blockchain_GetAccount);
            Register("Quras.Blockchain.GetValidators", Blockchain_GetValidators);
            Register("Quras.Blockchain.GetAsset", Blockchain_GetAsset);
            Register("Quras.Blockchain.GetContract", Blockchain_GetContract);
            Register("Quras.Header.GetHash", Header_GetHash);
            Register("Quras.Header.GetVersion", Header_GetVersion);
            Register("Quras.Header.GetPrevHash", Header_GetPrevHash);
            Register("Quras.Header.GetMerkleRoot", Header_GetMerkleRoot);
            Register("Quras.Header.GetTimestamp", Header_GetTimestamp);
            Register("Quras.Header.GetConsensusData", Header_GetConsensusData);
            Register("Quras.Header.GetNextConsensus", Header_GetNextConsensus);
            Register("Quras.Block.GetTransactionCount", Block_GetTransactionCount);
            Register("Quras.Block.GetTransactions", Block_GetTransactions);
            Register("Quras.Block.GetTransaction", Block_GetTransaction);
            Register("Quras.Transaction.GetHash", Transaction_GetHash);
            Register("Quras.Transaction.GetType", Transaction_GetType);
            Register("Quras.Transaction.GetAttributes", Transaction_GetAttributes);
            Register("Quras.Transaction.GetInputs", Transaction_GetInputs);
            Register("Quras.Transaction.GetOutputs", Transaction_GetOutputs);
            Register("Quras.Transaction.GetReferences", Transaction_GetReferences);
            Register("Quras.Attribute.GetUsage", Attribute_GetUsage);
            Register("Quras.Attribute.GetData", Attribute_GetData);
            Register("Quras.Input.GetHash", Input_GetHash);
            Register("Quras.Input.GetIndex", Input_GetIndex);
            Register("Quras.Output.GetAssetId", Output_GetAssetId);
            Register("Quras.Output.GetValue", Output_GetValue);
            Register("Quras.Output.GetScriptHash", Output_GetScriptHash);
            Register("Quras.Account.GetScriptHash", Account_GetScriptHash);
            Register("Quras.Account.GetVotes", Account_GetVotes);
            Register("Quras.Account.GetBalance", Account_GetBalance);
            Register("Quras.Asset.GetAssetId", Asset_GetAssetId);
            Register("Quras.Asset.GetAssetType", Asset_GetAssetType);
            Register("Quras.Asset.GetAmount", Asset_GetAmount);
            Register("Quras.Asset.GetAvailable", Asset_GetAvailable);
            Register("Quras.Asset.GetPrecision", Asset_GetPrecision);
            Register("Quras.Asset.GetOwner", Asset_GetOwner);
            Register("Quras.Asset.GetAdmin", Asset_GetAdmin);
            Register("Quras.Asset.GetIssuer", Asset_GetIssuer);
            Register("Quras.Contract.GetScript", Contract_GetScript);
            Register("Quras.Storage.GetContext", Storage_GetContext);
            Register("Quras.Storage.Get", Storage_Get);
            #region Old Pure APIs
            Register("Pure.Runtime.GetTrigger", Runtime_GetTrigger);
            Register("Pure.Runtime.CheckWitness", Runtime_CheckWitness);
            Register("Pure.Runtime.Notify", Runtime_Notify);
            Register("Pure.Runtime.Log", Runtime_Log);
            Register("Pure.Blockchain.GetHeight", Blockchain_GetHeight);
            Register("Pure.Blockchain.GetHeader", Blockchain_GetHeader);
            Register("Pure.Blockchain.GetBlock", Blockchain_GetBlock);
            Register("Pure.Blockchain.GetTransaction", Blockchain_GetTransaction);
            Register("Pure.Blockchain.GetAccount", Blockchain_GetAccount);
            Register("Pure.Blockchain.GetValidators", Blockchain_GetValidators);
            Register("Pure.Blockchain.GetAsset", Blockchain_GetAsset);
            Register("Pure.Blockchain.GetContract", Blockchain_GetContract);
            Register("Pure.Header.GetHash", Header_GetHash);
            Register("Pure.Header.GetVersion", Header_GetVersion);
            Register("Pure.Header.GetPrevHash", Header_GetPrevHash);
            Register("Pure.Header.GetMerkleRoot", Header_GetMerkleRoot);
            Register("Pure.Header.GetTimestamp", Header_GetTimestamp);
            Register("Pure.Header.GetConsensusData", Header_GetConsensusData);
            Register("Pure.Header.GetNextConsensus", Header_GetNextConsensus);
            Register("Pure.Block.GetTransactionCount", Block_GetTransactionCount);
            Register("Pure.Block.GetTransactions", Block_GetTransactions);
            Register("Pure.Block.GetTransaction", Block_GetTransaction);
            Register("Pure.Transaction.GetHash", Transaction_GetHash);
            Register("Pure.Transaction.GetType", Transaction_GetType);
            Register("Pure.Transaction.GetAttributes", Transaction_GetAttributes);
            Register("Pure.Transaction.GetInputs", Transaction_GetInputs);
            Register("Pure.Transaction.GetOutputs", Transaction_GetOutputs);
            Register("Pure.Transaction.GetReferences", Transaction_GetReferences);
            Register("Pure.Attribute.GetUsage", Attribute_GetUsage);
            Register("Pure.Attribute.GetData", Attribute_GetData);
            Register("Pure.Input.GetHash", Input_GetHash);
            Register("Pure.Input.GetIndex", Input_GetIndex);
            Register("Pure.Output.GetAssetId", Output_GetAssetId);
            Register("Pure.Output.GetValue", Output_GetValue);
            Register("Pure.Output.GetScriptHash", Output_GetScriptHash);
            Register("Pure.Account.GetScriptHash", Account_GetScriptHash);
            Register("Pure.Account.GetVotes", Account_GetVotes);
            Register("Pure.Account.GetBalance", Account_GetBalance);
            Register("Pure.Asset.GetAssetId", Asset_GetAssetId);
            Register("Pure.Asset.GetAssetType", Asset_GetAssetType);
            Register("Pure.Asset.GetAmount", Asset_GetAmount);
            Register("Pure.Asset.GetAvailable", Asset_GetAvailable);
            Register("Pure.Asset.GetPrecision", Asset_GetPrecision);
            Register("Pure.Asset.GetOwner", Asset_GetOwner);
            Register("Pure.Asset.GetAdmin", Asset_GetAdmin);
            Register("Pure.Asset.GetIssuer", Asset_GetIssuer);
            Register("Pure.Contract.GetScript", Contract_GetScript);
            Register("Pure.Storage.GetContext", Storage_GetContext);
            Register("Pure.Storage.Get", Storage_Get);
            #endregion
        }

        protected virtual bool Runtime_GetTrigger(ExecutionEngine engine)
        {
            ApplicationEngine app_engine = (ApplicationEngine)engine;
            engine.EvaluationStack.Push((int)app_engine.Trigger);
            return true;
        }

        protected bool CheckWitness(ExecutionEngine engine, UInt160 hash)
        {
            IVerifiable container = (IVerifiable)engine.ScriptContainer;
            UInt160[] _hashes_for_verifying = container.GetScriptHashesForVerifying();
            return _hashes_for_verifying.Contains(hash);
        }

        protected bool CheckWitness(ExecutionEngine engine, ECPoint pubkey)
        {
            return CheckWitness(engine, Contract.CreateSignatureRedeemScript(pubkey).ToScriptHash());
        }

        protected virtual bool Runtime_CheckWitness(ExecutionEngine engine)
        {
            byte[] hashOrPubkey = engine.EvaluationStack.Pop().GetByteArray();
            bool result;
            if (hashOrPubkey.Length == 20)
                result = CheckWitness(engine, new UInt160(hashOrPubkey));
            else if (hashOrPubkey.Length == 33)
                result = CheckWitness(engine, ECPoint.DecodePoint(hashOrPubkey, ECCurve.Secp256r1));
            else
                return false;
            engine.EvaluationStack.Push(result);
            return true;
        }

        protected virtual bool Runtime_Notify(ExecutionEngine engine)
        {
            StackItem state = engine.EvaluationStack.Pop();
            Notify?.Invoke(this, new NotifyEventArgs(engine.ScriptContainer, new UInt160(engine.CurrentContext.ScriptHash), state));
            return true;
        }

        protected virtual bool Runtime_Log(ExecutionEngine engine)
        {
            string message = Encoding.UTF8.GetString(engine.EvaluationStack.Pop().GetByteArray());
            Log?.Invoke(this, new LogEventArgs(engine.ScriptContainer, new UInt160(engine.CurrentContext.ScriptHash), message));
            return true;
        }

        protected virtual bool Blockchain_GetHeight(ExecutionEngine engine)
        {
            if (Blockchain.Default == null)
                engine.EvaluationStack.Push(0);
            else
                engine.EvaluationStack.Push(Blockchain.Default.Height);
            return true;
        }

        protected virtual bool Blockchain_GetHeader(ExecutionEngine engine)
        {
            byte[] data = engine.EvaluationStack.Pop().GetByteArray();
            Header header;
            if (data.Length <= 5)
            {
                uint height = (uint)new BigInteger(data);
                if (Blockchain.Default != null)
                    header = Blockchain.Default.GetHeader(height);
                else if (height == 0)
                    header = Blockchain.GenesisBlock.Header;
                else
                    header = null;
            }
            else if (data.Length == 32)
            {
                UInt256 hash = new UInt256(data);
                if (Blockchain.Default != null)
                    header = Blockchain.Default.GetHeader(hash);
                else if (hash == Blockchain.GenesisBlock.Hash)
                    header = Blockchain.GenesisBlock.Header;
                else
                    header = null;
            }
            else
            {
                return false;
            }
            engine.EvaluationStack.Push(StackItem.FromInterface(header));
            return true;
        }

        protected virtual bool Blockchain_GetBlock(ExecutionEngine engine)
        {
            byte[] data = engine.EvaluationStack.Pop().GetByteArray();
            Block block;
            if (data.Length <= 5)
            {
                uint height = (uint)new BigInteger(data);
                if (Blockchain.Default != null)
                    block = Blockchain.Default.GetBlock(height);
                else if (height == 0)
                    block = Blockchain.GenesisBlock;
                else
                    block = null;
            }
            else if (data.Length == 32)
            {
                UInt256 hash = new UInt256(data);
                if (Blockchain.Default != null)
                    block = Blockchain.Default.GetBlock(hash);
                else if (hash == Blockchain.GenesisBlock.Hash)
                    block = Blockchain.GenesisBlock;
                else
                    block = null;
            }
            else
            {
                return false;
            }
            engine.EvaluationStack.Push(StackItem.FromInterface(block));
            return true;
        }

        protected virtual bool Blockchain_GetTransaction(ExecutionEngine engine)
        {
            byte[] hash = engine.EvaluationStack.Pop().GetByteArray();
            Transaction tx = Blockchain.Default?.GetTransaction(new UInt256(hash));
            engine.EvaluationStack.Push(StackItem.FromInterface(tx));
            return true;
        }

        protected virtual bool Blockchain_GetAccount(ExecutionEngine engine)
        {
            byte[] hash = engine.EvaluationStack.Pop().GetByteArray();
            AccountState account = Blockchain.Default?.GetAccountState(new UInt160(hash));
            engine.EvaluationStack.Push(StackItem.FromInterface(account));
            return true;
        }

        protected virtual bool Blockchain_GetValidators(ExecutionEngine engine)
        {
            ECPoint[] validators = Blockchain.Default.GetValidators();
            engine.EvaluationStack.Push(validators.Select(p => (StackItem)p.EncodePoint(true)).ToArray());
            return true;
        }

        protected virtual bool Blockchain_GetAsset(ExecutionEngine engine)
        {
            byte[] hash = engine.EvaluationStack.Pop().GetByteArray();
            AssetState asset = Blockchain.Default?.GetAssetState(new UInt256(hash));
            engine.EvaluationStack.Push(StackItem.FromInterface(asset));
            return true;
        }

        protected virtual bool Blockchain_GetContract(ExecutionEngine engine)
        {
            UInt160 hash = new UInt160(engine.EvaluationStack.Pop().GetByteArray());
            ContractState contract = Blockchain.Default.GetContract(hash);
            if (contract == null) return false;
            engine.EvaluationStack.Push(StackItem.FromInterface(contract));
            return true;
        }

        protected virtual bool Header_GetHash(ExecutionEngine engine)
        {
            BlockBase header = engine.EvaluationStack.Pop().GetInterface<BlockBase>();
            if (header == null) return false;
            engine.EvaluationStack.Push(header.Hash.ToArray());
            return true;
        }

        protected virtual bool Header_GetVersion(ExecutionEngine engine)
        {
            BlockBase header = engine.EvaluationStack.Pop().GetInterface<BlockBase>();
            if (header == null) return false;
            engine.EvaluationStack.Push(header.Version);
            return true;
        }

        protected virtual bool Header_GetPrevHash(ExecutionEngine engine)
        {
            BlockBase header = engine.EvaluationStack.Pop().GetInterface<BlockBase>();
            if (header == null) return false;
            engine.EvaluationStack.Push(header.PrevHash.ToArray());
            return true;
        }

        protected virtual bool Header_GetMerkleRoot(ExecutionEngine engine)
        {
            BlockBase header = engine.EvaluationStack.Pop().GetInterface<BlockBase>();
            if (header == null) return false;
            engine.EvaluationStack.Push(header.MerkleRoot.ToArray());
            return true;
        }

        protected virtual bool Header_GetTimestamp(ExecutionEngine engine)
        {
            BlockBase header = engine.EvaluationStack.Pop().GetInterface<BlockBase>();
            if (header == null) return false;
            engine.EvaluationStack.Push(header.Timestamp);
            return true;
        }

        protected virtual bool Header_GetConsensusData(ExecutionEngine engine)
        {
            BlockBase header = engine.EvaluationStack.Pop().GetInterface<BlockBase>();
            if (header == null) return false;
            engine.EvaluationStack.Push(header.ConsensusData);
            return true;
        }

        protected virtual bool Header_GetNextConsensus(ExecutionEngine engine)
        {
            BlockBase header = engine.EvaluationStack.Pop().GetInterface<BlockBase>();
            if (header == null) return false;
            engine.EvaluationStack.Push(header.NextConsensus.ToArray());
            return true;
        }

        protected virtual bool Block_GetTransactionCount(ExecutionEngine engine)
        {
            Block block = engine.EvaluationStack.Pop().GetInterface<Block>();
            if (block == null) return false;
            engine.EvaluationStack.Push(block.Transactions.Length);
            return true;
        }

        protected virtual bool Block_GetTransactions(ExecutionEngine engine)
        {
            Block block = engine.EvaluationStack.Pop().GetInterface<Block>();
            if (block == null) return false;
            engine.EvaluationStack.Push(block.Transactions.Select(p => StackItem.FromInterface(p)).ToArray());
            return true;
        }

        protected virtual bool Block_GetTransaction(ExecutionEngine engine)
        {
            Block block = engine.EvaluationStack.Pop().GetInterface<Block>();
            int index = (int)engine.EvaluationStack.Pop().GetBigInteger();
            if (block == null) return false;
            if (index < 0 || index >= block.Transactions.Length) return false;
            Transaction tx = block.Transactions[index];
            engine.EvaluationStack.Push(StackItem.FromInterface(tx));
            return true;
        }

        protected virtual bool Transaction_GetHash(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push(tx.Hash.ToArray());
            return true;
        }

        protected virtual bool Transaction_GetType(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push((int)tx.Type);
            return true;
        }

        protected virtual bool Transaction_GetAttributes(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push(tx.Attributes.Select(p => StackItem.FromInterface(p)).ToArray());
            return true;
        }

        protected virtual bool Transaction_GetInputs(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push(tx.Inputs.Select(p => StackItem.FromInterface(p)).ToArray());
            return true;
        }

        protected virtual bool Transaction_GetOutputs(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push(tx.Outputs.Select(p => StackItem.FromInterface(p)).ToArray());
            return true;
        }

        protected virtual bool Transaction_GetReferences(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push(tx.Inputs.Select(p => StackItem.FromInterface(tx.References[p])).ToArray());
            return true;
        }

        protected virtual bool Attribute_GetUsage(ExecutionEngine engine)
        {
            TransactionAttribute attr = engine.EvaluationStack.Pop().GetInterface<TransactionAttribute>();
            if (attr == null) return false;
            engine.EvaluationStack.Push((int)attr.Usage);
            return true;
        }

        protected virtual bool Attribute_GetData(ExecutionEngine engine)
        {
            TransactionAttribute attr = engine.EvaluationStack.Pop().GetInterface<TransactionAttribute>();
            if (attr == null) return false;
            engine.EvaluationStack.Push(attr.Data);
            return true;
        }

        protected virtual bool Input_GetHash(ExecutionEngine engine)
        {
            CoinReference input = engine.EvaluationStack.Pop().GetInterface<CoinReference>();
            if (input == null) return false;
            engine.EvaluationStack.Push(input.PrevHash.ToArray());
            return true;
        }

        protected virtual bool Input_GetIndex(ExecutionEngine engine)
        {
            CoinReference input = engine.EvaluationStack.Pop().GetInterface<CoinReference>();
            if (input == null) return false;
            engine.EvaluationStack.Push((int)input.PrevIndex);
            return true;
        }

        protected virtual bool Output_GetAssetId(ExecutionEngine engine)
        {
            TransactionOutput output = engine.EvaluationStack.Pop().GetInterface<TransactionOutput>();
            if (output == null) return false;
            engine.EvaluationStack.Push(output.AssetId.ToArray());
            return true;
        }

        protected virtual bool Output_GetValue(ExecutionEngine engine)
        {
            TransactionOutput output = engine.EvaluationStack.Pop().GetInterface<TransactionOutput>();
            if (output == null) return false;
            engine.EvaluationStack.Push(output.Value.GetData());
            return true;
        }

        protected virtual bool Output_GetScriptHash(ExecutionEngine engine)
        {
            TransactionOutput output = engine.EvaluationStack.Pop().GetInterface<TransactionOutput>();
            if (output == null) return false;
            engine.EvaluationStack.Push(output.ScriptHash.ToArray());
            return true;
        }

        protected virtual bool Account_GetScriptHash(ExecutionEngine engine)
        {
            AccountState account = engine.EvaluationStack.Pop().GetInterface<AccountState>();
            if (account == null) return false;
            engine.EvaluationStack.Push(account.ScriptHash.ToArray());
            return true;
        }

        protected virtual bool Account_GetVotes(ExecutionEngine engine)
        {
            AccountState account = engine.EvaluationStack.Pop().GetInterface<AccountState>();
            if (account == null) return false;
            engine.EvaluationStack.Push(account.Votes.Select(p => (StackItem)p.EncodePoint(true)).ToArray());
            return true;
        }

        protected virtual bool Account_GetBalance(ExecutionEngine engine)
        {
            AccountState account = engine.EvaluationStack.Pop().GetInterface<AccountState>();
            UInt256 asset_id = new UInt256(engine.EvaluationStack.Pop().GetByteArray());
            if (account == null) return false;
            Fixed8 balance = account.Balances.TryGetValue(asset_id, out Fixed8 value) ? value : Fixed8.Zero;
            engine.EvaluationStack.Push(balance.GetData());
            return true;
        }

        protected virtual bool Asset_GetAssetId(ExecutionEngine engine)
        {
            AssetState asset = engine.EvaluationStack.Pop().GetInterface<AssetState>();
            if (asset == null) return false;
            engine.EvaluationStack.Push(asset.AssetId.ToArray());
            return true;
        }

        protected virtual bool Asset_GetAssetType(ExecutionEngine engine)
        {
            AssetState asset = engine.EvaluationStack.Pop().GetInterface<AssetState>();
            if (asset == null) return false;
            engine.EvaluationStack.Push((int)asset.AssetType);
            return true;
        }

        protected virtual bool Asset_GetAmount(ExecutionEngine engine)
        {
            AssetState asset = engine.EvaluationStack.Pop().GetInterface<AssetState>();
            if (asset == null) return false;
            engine.EvaluationStack.Push(asset.Amount.GetData());
            return true;
        }

        protected virtual bool Asset_GetAvailable(ExecutionEngine engine)
        {
            AssetState asset = engine.EvaluationStack.Pop().GetInterface<AssetState>();
            if (asset == null) return false;
            engine.EvaluationStack.Push(asset.Available.GetData());
            return true;
        }

        protected virtual bool Asset_GetPrecision(ExecutionEngine engine)
        {
            AssetState asset = engine.EvaluationStack.Pop().GetInterface<AssetState>();
            if (asset == null) return false;
            engine.EvaluationStack.Push((int)asset.Precision);
            return true;
        }

        protected virtual bool Asset_GetOwner(ExecutionEngine engine)
        {
            AssetState asset = engine.EvaluationStack.Pop().GetInterface<AssetState>();
            if (asset == null) return false;
            engine.EvaluationStack.Push(asset.Owner.EncodePoint(true));
            return true;
        }

        protected virtual bool Asset_GetAdmin(ExecutionEngine engine)
        {
            AssetState asset = engine.EvaluationStack.Pop().GetInterface<AssetState>();
            if (asset == null) return false;
            engine.EvaluationStack.Push(asset.Admin.ToArray());
            return true;
        }

        protected virtual bool Asset_GetIssuer(ExecutionEngine engine)
        {
            AssetState asset = engine.EvaluationStack.Pop().GetInterface<AssetState>();
            if (asset == null) return false;
            engine.EvaluationStack.Push(asset.Issuer.ToArray());
            return true;
        }

        protected virtual bool Contract_GetScript(ExecutionEngine engine)
        {
            ContractState contract = engine.EvaluationStack.Pop().GetInterface<ContractState>();
            if (contract == null) return false;
            engine.EvaluationStack.Push(contract.Script);
            return true;
        }

        protected virtual bool Storage_GetContext(ExecutionEngine engine)
        {
            engine.EvaluationStack.Push(StackItem.FromInterface(new StorageContext
            {
                ScriptHash = new UInt160(engine.CurrentContext.ScriptHash)
            }));
            return true;
        }

        protected virtual bool Storage_Get(ExecutionEngine engine)
        {
            StorageContext context = engine.EvaluationStack.Pop().GetInterface<StorageContext>();
            ContractState contract = Blockchain.Default.GetContract(context.ScriptHash);
            if (contract == null) return false;
            if (!contract.HasStorage) return false;
            byte[] key = engine.EvaluationStack.Pop().GetByteArray();
            StorageItem item = Blockchain.Default.GetStorageItem(new StorageKey
            {
                ScriptHash = context.ScriptHash,
                Key = key
            });
            engine.EvaluationStack.Push(item?.Value ?? new byte[0]);
            return true;
        }
    }
}
