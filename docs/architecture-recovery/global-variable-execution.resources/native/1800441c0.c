
undefined4 FUN_1800441c0(longlong param_1,int param_2)

{
  undefined8 uVar1;
  char cVar2;
  int iVar3;
  
  if ((*(char *)(param_1 + 0x23) != '\0') && (*(longlong *)(param_1 + 0x558) != 0)) {
    cVar2 = FUN_18001dbc0(2);
    if (cVar2 != '\0') {
      uVar1 = FUN_18001db50();
      FUN_180015b40(uVar1,2,&UNK_18021f910,&UNK_18021f8e8,0x462);
    }
    iVar3 = FUN_1800da1c0(*(undefined8 *)(param_1 + 0x558),(longlong)param_2);
    if (iVar3 == 0x102) {
      cVar2 = FUN_18001dbc0(2);
      if (cVar2 != '\0') {
        uVar1 = FUN_18001db50();
        FUN_180015b40(uVar1,2,&UNK_18021f938,&UNK_18021f8e8,0x465);
      }
      return 0;
    }
    cVar2 = FUN_18001dbc0(2);
    if (cVar2 != '\0') {
      uVar1 = FUN_18001db50();
      FUN_180015b40(uVar1,2,&UNK_18021f958,&UNK_18021f8e8,0x468);
    }
  }
  return 1;
}

