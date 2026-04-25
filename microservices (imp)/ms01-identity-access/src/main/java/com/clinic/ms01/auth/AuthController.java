package com.clinic.ms01.auth;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/auth")
public class AuthController {

    @PostMapping("/login")
    public ResponseEntity<String> login() {
        // US-042 placeholder: replace with JWT authentication flow.
        return ResponseEntity.ok("Login endpoint scaffold");
    }

    @PostMapping("/logout")
    public ResponseEntity<String> logout() {
        // US-042 placeholder: replace with token/session invalidation.
        return ResponseEntity.ok("Logout endpoint scaffold");
    }

    @GetMapping("/me")
    public ResponseEntity<String> me() {
        return ResponseEntity.ok("Current user profile scaffold");
    }
}
