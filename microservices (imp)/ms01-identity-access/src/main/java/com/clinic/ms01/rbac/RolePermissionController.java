package com.clinic.ms01.rbac;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/rbac")
public class RolePermissionController {

    @GetMapping("/roles")
    public ResponseEntity<String> listRoles() {
        // US-043/US-006 placeholder: role and permission matrix source of truth.
        return ResponseEntity.ok("RBAC role list scaffold");
    }
}
