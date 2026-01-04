#!/bin/bash

echo "=========================================="
echo "Testing Dynamic Claims Configuration"
echo "=========================================="

echo ""
echo "1. Checking hc-blazor container status..."
docker ps --filter "name=hc-blazor" --format "table {{.Names}}\t{{.Status}}"

echo ""
echo "2. Checking hc-api container status..."
docker ps --filter "name=hc-api" --format "table {{.Names}}\t{{.Status}}"

echo ""
echo "3. Checking RemoteServices configuration in hc-blazor..."
docker exec hc-blazor printenv | grep -i "RemoteServices__Default__BaseUrl" || echo "Not found"

echo ""
echo "4. Testing API endpoint from hc-blazor container..."
echo "   (This requires authentication token, so it may fail)"
docker exec hc-blazor curl -s -o /dev/null -w "%{http_code}" http://hc-api:8080/api/abp/application-configuration || echo "Failed to connect"

echo ""
echo "5. Checking Dynamic Claims configuration in logs..."
echo "   hc-blazor logs (last 50 lines, filtering for Dynamic Claims):"
docker logs hc-blazor --tail 50 2>&1 | grep -i "dynamic\|claim\|permission" | tail -10 || echo "No relevant logs found"

echo ""
echo "6. Checking hc-api logs for application-configuration requests..."
docker logs hc-api --tail 100 2>&1 | grep -i "application-configuration\|permission" | tail -10 || echo "No relevant logs found"

echo ""
echo "=========================================="
echo "Manual Testing Steps:"
echo "=========================================="
echo "1. Open browser DevTools → Network tab"
echo "2. Login to https://dev.benhvien199.vn"
echo "3. Look for request: /api/abp/application-configuration"
echo "4. Check response contains 'auth.permissions' object"
echo "5. Verify permissions are present in the response"
echo ""
echo "If permissions are missing from API response:"
echo "  - Check database: SELECT COUNT(*) FROM \"AbpRoleClaims\" WHERE \"ClaimType\" = 'permission'"
echo "  - Verify admin user has admin role"
echo "  - Restart services: docker-compose restart hc-blazor hc-api"
echo "=========================================="

