#!/bin/bash
# Create pages from template
template="DocumentTypes.razor"

# Array of page configs: route, permission, type, title_key
pages=(
  "/sectors|HCPermissions.MasterDatas.SectorDefault|LINH_VUC_VB|Sector"
  "/urgency-levels|HCPermissions.MasterDatas.UrgencyLevelDefault|MUC_DO_KHAN|UrgencyLevel"
  "/confidentiality-levels|HCPermissions.MasterDatas.ConfidentialityLevelDefault|MUC_DO_MAT|ConfidentialityLevel"
  "/processing-methods|HCPermissions.MasterDatas.ProcessingMethodDefault|HINH_THUC_XL|ProcessingMethod"
  "/document-status|HCPermissions.MasterDatas.DocumentStatusDefault|TRANG_THAI_VB|DocumentStatus"
  "/signing-methods|HCPermissions.MasterDatas.SigningMethodDefault|LOAI_KY|SigningMethod"
  "/even-types|HCPermissions.MasterDatas.EventTypeDefault|LOAI_SU_KIEN|EventType"
)

for page_config in "${pages[@]}"; do
  IFS='|' read -r route permission type title_key <<< "$page_config"
  filename=$(basename "$route").razor
  filename="${filename//-/_}"
  
  echo "Creating $filename..."
done
