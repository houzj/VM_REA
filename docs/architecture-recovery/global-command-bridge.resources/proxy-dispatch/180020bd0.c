
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_PROXY_RecvServerCommand(void)

{
                    /* 0x20bd0  3  VM_PROXY_RecvServerCommand */
  if ((_DAT_180198808 & 1) == 0) {
    _DAT_180198808 = _DAT_180198808 | 1;
    func_0x00018011e414(&DAT_180198620,0,0x1e8);
    FUN_180047f80();
    FUN_18011dbb0(FUN_18012c2c0);
  }
  FUN_180049cb0();
  return;
}

