<template>
  <b-modal :id="id" :title="$t('user.session.signOutAll')">
    <p>
      {{ $t('user.session.confirmAll') }}
      <br />
      {{ $t(isCurrent ? 'user.session.confirmAllCurrent' : 'user.session.confirmAllOther') }}
      <br />
      <span class="text-danger">{{ user.fullName ? `${user.fullName} (${user.username})` : user.username }}</span>
    </p>
    <slot />
    <template #modal-footer="{ cancel, ok }">
      <icon-button icon="ban" text="actions.cancel" @click="cancel()" />
      <icon-button :disabled="loading" icon="sign-out-alt" :loading="loading" text="user.session.signOutAll" variant="danger" @click="$emit('ok', ok)" />
    </template>
  </b-modal>
</template>

<script>
export default {
  name: 'SignOutModal',
  props: {
    id: {
      type: String,
      default: 'signOutAll'
    },
    isCurrent: {
      type: Boolean,
      default: false
    },
    loading: {
      type: Boolean,
      default: false
    },
    user: {
      type: Object,
      required: true
    }
  }
}
</script>
