
void FUN_1800b9810(longlong param_1)

{
  undefined8 uVar1;
  char cVar2;
  uint uVar3;
  int iVar4;
  
  uVar3 = WaitForSingleObject(*(undefined8 *)(param_1 + 0x128),0xffffffff);
  if (((uVar3 & 0xffffff7f) != 0) &&
     (((iVar4 = 0x102, uVar3 == 0x102 || (iVar4 = GetLastError(), iVar4 != 0)) &&
      (cVar2 = FUN_18001dbc0(4), cVar2 != '\0')))) {
    uVar1 = FUN_18001db50();
    FUN_180015b40(uVar1,4,&UNK_180229df0,&UNK_180229da0,0x186,iVar4);
  }
  iVar4 = ResetEvent(*(undefined8 *)(param_1 + 0x128));
  if (iVar4 != 0) {
    return;
  }
                    /* WARNING: Could not recover jumptable at 0x0001800b98a6. Too many branches */
                    /* WARNING: Treating indirect jump as call */
  GetLastError();
  return;
}

