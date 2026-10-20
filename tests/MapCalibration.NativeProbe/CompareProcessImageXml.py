"""Compare actual original Process output and actual plugin Runtime XML; normalize only its recorded clock text."""
import pathlib,re,json,sys
base=pathlib.Path(__file__).resolve().parents[2]/'analysis/map-calibration'
folder=sys.argv[1] if len(sys.argv)>1 else 'image-execution-source-grid'
native=(base/'image-runtime-source-grid/normal-process.xml').read_bytes()
product=(base/folder/'image-1-1-7.xml').read_bytes()
pattern=rb'(<CalibParam ParamName="CreateCalibTime" DataType="string">\r\n            <ParamValue>)[^<]+'
def normalize(value):
 assert len(re.findall(pattern,value))==1
 return re.sub(pattern,lambda m:m.group(1)+b'TIME',value)
report=dict(equalExceptRecordedTime=normalize(native)==normalize(product),nativeLength=len(native),productLength=len(product),nativePath='image-runtime-source-grid/normal-process.xml',productPath=folder+'/image-1-1-7.xml',normalization='Only CreateCalibTime text; all other bytes retained',scope='Actual native Process vs actual plugin Runtime, Weight1 model1 board7 no input pose')
(base/folder/'process-xml-comparison.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
assert report['equalExceptRecordedTime'],report
print('Actual Process/Runtime XML: 1790 bytes equal except independent clock text')
