﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Pure.Implementations.Wallets.EntityFramework
{
    internal class WalletDataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<JSCoin> JSCoins { get; set; }
        public DbSet<RCTCoin> RCTCoins { get; set; }

        private readonly string filename;

        public WalletDataContext(string filename)
        {
            this.filename = filename;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            SqliteConnectionStringBuilder sb = new SqliteConnectionStringBuilder();
            sb.DataSource = filename;
            optionsBuilder.UseSqlite(sb.ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>().ToTable(nameof(Account));
            modelBuilder.Entity<Account>().HasKey(p => p.PublicKeyHash);
            modelBuilder.Entity<Account>().Property(p => p.PrivateKeyEncrypted).HasColumnType("VarBinary").HasMaxLength(96).IsRequired();
            modelBuilder.Entity<Account>().Property(p => p.PublicKeyHash).HasColumnType("Binary").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Account>().Property(p => p.PrivateViewKeyEncrypted).HasColumnType("VarBinary").HasMaxLength(96).IsRequired(false);
            modelBuilder.Entity<Address>().ToTable(nameof(Address));
            modelBuilder.Entity<Address>().HasKey(p => p.ScriptHash);
            modelBuilder.Entity<Address>().Property(p => p.ScriptHash).HasColumnType("Binary").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Contract>().ToTable(nameof(Contract));
            modelBuilder.Entity<Contract>().HasKey(p => p.ScriptHash);
            modelBuilder.Entity<Contract>().HasIndex(p => p.PublicKeyHash);
            modelBuilder.Entity<Contract>().HasOne(p => p.Account).WithMany().HasForeignKey(p => p.PublicKeyHash).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Contract>().HasOne(p => p.Address).WithMany().HasForeignKey(p => p.ScriptHash).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Contract>().Property(p => p.RawData).HasColumnType("VarBinary").IsRequired();
            modelBuilder.Entity<Contract>().Property(p => p.ScriptHash).HasColumnType("Binary").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Contract>().Property(p => p.PublicKeyHash).HasColumnType("Binary").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Key>().ToTable(nameof(Key));
            modelBuilder.Entity<Key>().HasKey(p => p.Name);
            modelBuilder.Entity<Key>().Property(p => p.Name).HasColumnType("VarChar").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Key>().Property(p => p.Value).HasColumnType("VarBinary").IsRequired();
            modelBuilder.Entity<Transaction>().ToTable(nameof(Transaction));
            modelBuilder.Entity<Transaction>().HasKey(p => p.Hash);
            modelBuilder.Entity<Transaction>().HasIndex(p => p.Type);
            modelBuilder.Entity<Transaction>().HasIndex(p => p.Height);
            modelBuilder.Entity<Transaction>().Property(p => p.Hash).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<Transaction>().Property(p => p.Type).IsRequired();
            modelBuilder.Entity<Transaction>().Property(p => p.RawData).HasColumnType("VarBinary").IsRequired();
            modelBuilder.Entity<Transaction>().Property(p => p.Height);
            modelBuilder.Entity<Transaction>().Property(p => p.Time).IsRequired();
            modelBuilder.Entity<Coin>().ToTable(nameof(Coin));
            modelBuilder.Entity<Coin>().HasKey(p => new { p.TxId, p.Index });
            modelBuilder.Entity<Coin>().HasIndex(p => p.AssetId);
            modelBuilder.Entity<Coin>().HasIndex(p => p.ScriptHash);
            modelBuilder.Entity<Coin>().HasOne(p => p.Address).WithMany().HasForeignKey(p => p.ScriptHash).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Coin>().Property(p => p.TxId).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<Coin>().Property(p => p.Index).IsRequired();
            modelBuilder.Entity<Coin>().Property(p => p.AssetId).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<Coin>().Property(p => p.ScriptHash).HasColumnType("Binary").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Coin>().Property(p => p.State).IsRequired();

            modelBuilder.Entity<JSCoin>().ToTable(nameof(JSCoin));
            modelBuilder.Entity<JSCoin>().HasKey(p => new { p.TxId, p.JsId, p.Index });
            modelBuilder.Entity<JSCoin>().HasIndex(p => p.AssetId);
            modelBuilder.Entity<JSCoin>().HasIndex(p => p.ScriptHash);
            modelBuilder.Entity<JSCoin>().HasOne(p => p.Address).WithMany().HasForeignKey(p => p.ScriptHash).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JSCoin>().Property(p => p.TxId).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.JsId).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.Index).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.AssetId).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.ScriptHash).HasColumnType("Binary").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.r).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.rho).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.Witness).HasColumnType("VarBinary").IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.WitnessHeight).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.CMTreeHeight).IsRequired();
            modelBuilder.Entity<JSCoin>().Property(p => p.State).IsRequired();

            modelBuilder.Entity<RCTCoin>().ToTable(nameof(RCTCoin));
            modelBuilder.Entity<RCTCoin>().HasKey(p => new { p.TxId, p.RctID, p.Index });
            modelBuilder.Entity<RCTCoin>().HasIndex(p => p.AssetId);
            modelBuilder.Entity<RCTCoin>().HasIndex(p => p.PubKey);
            modelBuilder.Entity<RCTCoin>().Property(p => p.TxId).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<RCTCoin>().Property(p => p.RctID).IsRequired();
            modelBuilder.Entity<RCTCoin>().Property(p => p.Index).IsRequired();
            modelBuilder.Entity<RCTCoin>().Property(p => p.AssetId).HasColumnType("Binary").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<RCTCoin>().Property(p => p.PubKey).HasColumnType("Binary").HasMaxLength(33).IsRequired();
            modelBuilder.Entity<RCTCoin>().Property(p => p.TxRCTHash).HasColumnType("Binary").HasMaxLength(33).IsRequired();
            modelBuilder.Entity<RCTCoin>().Property(p => p.ScriptHash).HasColumnType("Binary").HasMaxLength(20).IsRequired();
        }
    }
}
