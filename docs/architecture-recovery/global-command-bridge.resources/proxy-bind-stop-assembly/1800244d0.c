
void FUN_1800244d0(longlong param_1,undefined4 param_2)

{
  if (*(code **)(param_1 + 0x120) != (code *)0x0) {
                    /* WARNING: Could not recover jumptable at 0x0001800244de. Too many branches */
                    /* WARNING: Treating indirect jump as call */
    (**(code **)(param_1 + 0x120))(param_2);
    return;
  }
  return;
}

